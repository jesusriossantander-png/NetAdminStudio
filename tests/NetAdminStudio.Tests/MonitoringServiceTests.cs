using System.Net;
using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Application.Monitoring;
using NetAdminStudio.Domain.Alerts;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Tests;

public sealed class MonitoringServiceTests
{
    private sealed class FakeProbe(bool success) : INetworkProbe
    {
        public Task<ProbeResult> PingAsync(string host, CancellationToken ct) =>
            Task.FromResult(new ProbeResult(success, success ? 1.0 : null, null));
    }

    private sealed class FakeScanner(IReadOnlyList<int> openPorts) : INetworkScanner
    {
        public Task<HostProbe> ProbeHostAsync(IPAddress ip, CancellationToken ct) =>
            Task.FromResult(new HostProbe(false, null, null));

        public Task<IReadOnlyList<int>> ScanPortsAsync(
            IPAddress ip, IReadOnlyList<int> ports, CancellationToken ct) =>
            Task.FromResult(openPorts);
    }

    private sealed class InMemoryAssets : IAssetRepository
    {
        public readonly List<NetworkAsset> Items = new();
        public Task<IReadOnlyList<NetworkAsset>> GetAllAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<NetworkAsset>>(Items);
        public Task UpsertAsync(NetworkAsset asset, CancellationToken ct) => Task.CompletedTask;
    }

    private sealed class InMemoryAlerts : IAlertRepository
    {
        public readonly List<Alert> Added = new();
        public Task<IReadOnlyList<Alert>> GetOpenAsync(CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<Alert>>(Added);
        public Task<Alert?> GetAsync(Guid id, CancellationToken ct) =>
            Task.FromResult<Alert?>(null);
        public Task AddAsync(Alert alert, CancellationToken ct) { Added.Add(alert); return Task.CompletedTask; }
        public Task SaveAsync(Alert alert, CancellationToken ct) => Task.CompletedTask;
    }

    private static NetworkAsset OnlineAssetWithPorts(params int[] ports)
    {
        var asset = new NetworkAsset { Name = "PC", IpAddress = "10.0.0.5", OpenPorts = ports };
        asset.RecordPresence(true, 1.0, DateTimeOffset.UtcNow);
        return asset;
    }

    [Fact]
    public async Task PingFailsButPortOpen_StaysOnline_NoAlert()
    {
        var assets = new InMemoryAssets { Items = { OnlineAssetWithPorts(445) } };
        var alerts = new InMemoryAlerts();
        var service = new MonitoringService(assets, alerts, new FakeProbe(false), new FakeScanner(new[] { 445 }));

        await service.MonitorOnceAsync(CancellationToken.None);

        Assert.Equal(OperationalState.Online, assets.Items[0].State);
        Assert.Empty(alerts.Added);
    }

    [Fact]
    public async Task PingFailsAndNoPortsRespond_GoesOffline_CreatesAlert()
    {
        var assets = new InMemoryAssets { Items = { OnlineAssetWithPorts(445) } };
        var alerts = new InMemoryAlerts();
        var service = new MonitoringService(assets, alerts, new FakeProbe(false), new FakeScanner(Array.Empty<int>()));

        await service.MonitorOnceAsync(CancellationToken.None);

        Assert.Equal(OperationalState.Offline, assets.Items[0].State);
        Assert.Single(alerts.Added);
    }
}
