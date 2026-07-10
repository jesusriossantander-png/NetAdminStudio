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
}
