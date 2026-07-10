using System.Net;
using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Application.Networking;

/// <summary>Un host descubierto por el escaneo, con toda su información identificada.</summary>
public sealed record DiscoveredHost(
    string Ip, string? Hostname, string? Mac, string? Vendor,
    IReadOnlyList<int> OpenPorts, AssetType Type, double? LatencyMs);

/// <summary>Progreso de un escaneo en curso.</summary>
public sealed record ScanProgress(int Total, int Completed, int Found);

/// <summary>
/// Orquesta el descubrimiento de red en 4 fases: barrido (ping) → identidad (DNS+ARP+OUI)
/// → puertos → clasificación. Persiste cada host encontrado como <see cref="NetworkAsset"/>.
/// </summary>
public sealed class NetworkDiscoveryEngine(
    INetworkScanner scanner,
    IArpTable arp,
    IVendorLookup vendorLookup,
    IAssetRepository assets)
{
    private const int PingConcurrency = 32;

    public async Task<IReadOnlyList<DiscoveredHost>> ScanAsync(
        string cidr, IProgress<ScanProgress>? progress, CancellationToken ct)
    {
        var hosts = Cidr.Hosts(cidr);
        var arpMap = await arp.GetAllAsync(ct);

        var found = new List<DiscoveredHost>();
        var completed = 0;
        using var gate = new SemaphoreSlim(PingConcurrency);

        var tasks = hosts.Select(async ip =>
        {
            await gate.WaitAsync(ct);
            try
            {
                var probe = await scanner.ProbeHostAsync(ip, ct);
                if (probe.Reachable)
                {
                    var openPorts = await scanner.ScanPortsAsync(ip, ScanPorts.Default, ct);
                    arpMap.TryGetValue(ip.ToString(), out var mac);
                    var vendor = vendorLookup.ResolveVendor(mac);
                    var type = AssetClassifier.Classify(openPorts, vendor);

                    var host = new DiscoveredHost(
                        ip.ToString(), probe.Hostname, mac, vendor,
                        openPorts, type, probe.LatencyMs);

                    lock (found) found.Add(host);
                    await PersistAsync(host, ct);
                }
            }
            finally
            {
                gate.Release();
                var done = Interlocked.Increment(ref completed);
                int foundCount;
                lock (found) foundCount = found.Count;
                progress?.Report(new ScanProgress(hosts.Count, done, foundCount));
            }
        });

        await Task.WhenAll(tasks);
        return found.OrderBy(h => h.Ip).ToList();
    }

    private async Task PersistAsync(DiscoveredHost host, CancellationToken ct)
    {
        var existing = (await assets.GetAllAsync(ct))
            .FirstOrDefault(a => a.IpAddress == host.Ip);

        var asset = existing ?? new NetworkAsset { Name = host.Hostname ?? host.Ip };
        asset.IpAddress = host.Ip;
        if (existing is null && host.Hostname is not null)
            asset.Name = host.Hostname;

        asset.RecordDiscovery(
            host.Hostname, host.Mac, host.Vendor,
            host.OpenPorts, host.Type, host.LatencyMs, DateTimeOffset.UtcNow);

        await assets.UpsertAsync(asset, ct);
    }
}
