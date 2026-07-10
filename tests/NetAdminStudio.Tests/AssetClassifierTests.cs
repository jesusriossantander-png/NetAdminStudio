using NetAdminStudio.Application.Networking;
using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Tests;

public sealed class AssetClassifierTests
{
    [Fact]
    public void PrinterPorts_ClassifiedAsPrinter() =>
        Assert.Equal(AssetType.Printer,
            AssetClassifier.Classify(new[] { 9100, 631 }, null));

    [Fact]
    public void SnmpWithNetworkVendor_ClassifiedAsSwitch() =>
        Assert.Equal(AssetType.Switch,
            AssetClassifier.Classify(new[] { 161, 80 }, "TP-Link"));

    [Fact]
    public void SmbAndRdp_ClassifiedAsWorkstation() =>
        Assert.Equal(AssetType.Workstation,
            AssetClassifier.Classify(new[] { 445, 3389 }, "Dell"));

    [Fact]
    public void NoSignals_ClassifiedAsUnknown() =>
        Assert.Equal(AssetType.Unknown,
            AssetClassifier.Classify(Array.Empty<int>(), null));
}
