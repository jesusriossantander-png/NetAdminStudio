using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Tests;

public sealed class NetworkAssetTests
{
    [Fact]
    public void RecordPresence_MarksAssetOnline()
    {
        var asset = new NetworkAsset
        {
            Name = "Switch",
            Type = AssetType.Switch
        };

        asset.RecordPresence(true, 4.2, DateTimeOffset.UtcNow);

        Assert.Equal(OperationalState.Online, asset.State);
        Assert.Equal(4.2, asset.LatencyMs);
    }

    [Fact]
    public void RecordPresence_MarksAssetOffline()
    {
        var asset = new NetworkAsset
        {
            Name = "Router",
            Type = AssetType.Router
        };

        asset.RecordPresence(false, null, DateTimeOffset.UtcNow);

        Assert.Equal(OperationalState.Offline, asset.State);
    }

    [Fact]
    public void RecordDiscovery_SetsDiscoveryFieldsAndOnline()
    {
        var asset = new NetworkAsset { Name = "PC-01", Type = AssetType.Unknown };
        var now = DateTimeOffset.UtcNow;

        asset.RecordDiscovery("PC-01.local", "AA-BB-CC-11-22-33", "Dell",
            new[] { 445, 3389 }, AssetType.Workstation, 3.0, now);

        Assert.Equal(OperationalState.Online, asset.State);
        Assert.Equal("PC-01.local", asset.Hostname);
        Assert.Equal("Dell", asset.Vendor);
        Assert.Equal(AssetType.Workstation, asset.Type);
        Assert.Equal(new[] { 445, 3389 }, asset.OpenPorts);
        Assert.Equal("discovery", asset.Origin);
        Assert.Equal(now, asset.FirstSeenAt);
    }
}
