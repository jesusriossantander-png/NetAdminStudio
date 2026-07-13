using System.Net;
using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Domain.Alerts;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Application.Monitoring;

public sealed class MonitoringService(
    IAssetRepository assets,
    IAlertRepository alerts,
    INetworkProbe probe,
    INetworkScanner scanner)
{
    public async Task MonitorOnceAsync(CancellationToken ct)
    {
        foreach (var asset in await assets.GetAllAsync(ct))
        {
            if (string.IsNullOrWhiteSpace(asset.IpAddress))
                continue;

            var wasOnline = asset.State == OperationalState.Online;
            var ping = await probe.PingAsync(asset.IpAddress, ct);

            var reachable = ping.Success;
            double? latency = ping.DurationMs;

            // Muchos equipos bloquean ICMP (ping) pero siguen vivos. Si el ping falla
            // y el activo tenía puertos abiertos conocidos, verificamos por TCP antes
            // de declararlo caído (evita falsos positivos de "offline").
            if (!reachable && asset.OpenPorts.Count > 0
                && IPAddress.TryParse(asset.IpAddress, out var ip))
            {
                var open = await scanner.ScanPortsAsync(ip, asset.OpenPorts, ct);
                if (open.Count > 0)
                {
                    reachable = true;
                    latency = null;
                }
            }

            asset.RecordPresence(reachable, latency, DateTimeOffset.UtcNow);
            await assets.UpsertAsync(asset, ct);

            if (!reachable && wasOnline)
            {
                await alerts.AddAsync(new Alert
                {
                    Title = $"{asset.Name} dejó de responder",
                    SourceType = "NetworkAsset",
                    SourceId = asset.Id,
                    Severity = AlertSeverity.Critical
                }, ct);
            }
        }
    }
}
