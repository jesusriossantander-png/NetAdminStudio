using System.Net;
using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Application.Networking;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Tests;

public sealed class NetworkDiscoveryEngineTests
{
    private sealed class FakeScanner : INetworkScanner
    {
        public Task<HostProbe> ProbeHostAsync(IPAddress ip, CancellationToken ct)
        {
            // Solo .1 responde.
            var reachable = ip.ToString().EndsWith(".1");
            return Task.FromResult(new HostProbe(
                reachable, reachable ? "router.local" : null, reachable ? 2.0 : null));
        }

        public Task<IReadOnlyList<int>> ScanPortsAsync(
            IPAddress ip, IReadOnlyList<int> ports, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<int>>(new[] { 80, 161 });
    }

    private sealed class FakeArp : IArpTable
    {
        public Task<IReadOnlyDictionary<string, string>> GetAllAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyDictionary<string, string>>(
                new Dictionary<string, string> { ["192.168.0.1"] = "50-C7-BF-11-22-33" });
    }

    private sealed class FakeVendor : IVendorLookup
    {
        public string? ResolveVendor(string? mac) => mac is null ? null : "TP-Link";
    }

    private sealed class InMemoryAssets : IAssetRepository
    {
        public readonly List<NetworkAsset> Saved = new();

        public Task<IReadOnlyList<NetworkAsset>> GetAllAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<NetworkAsset>>(Saved);

        public Task UpsertAsync(NetworkAsset asset, CancellationToken ct)
        {
            Saved.RemoveAll(a => a.IpAddress == asset.IpAddress);
            Saved.Add(asset);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task ScanAsync_DiscoversReachableHostAndPersists()
    {
        var assets = new InMemoryAssets();
        var engine = new NetworkDiscoveryEngine(
            new FakeScanner(), new FakeArp(), new FakeVendor(), assets);

        var found = await engine.ScanAsync("192.168.0.0/30", null, CancellationToken.None);

        var host = Assert.Single(found);
        Assert.Equal("192.168.0.1", host.Ip);
        Assert.Equal("TP-Link", host.Vendor);
        Assert.Equal(AssetType.Switch, host.Type);          // 161 + TP-Link
        Assert.Single(assets.Saved);
        Assert.Equal("discovery", assets.Saved[0].Origin);
    }
}
