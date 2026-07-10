using NetAdminStudio.Infrastructure.Networking;

namespace NetAdminStudio.Tests;

public sealed class OuiVendorLookupTests
{
    private readonly OuiVendorLookup _lookup = new();

    [Theory]
    [InlineData("50-C7-BF-11-22-33", "TP-Link")]   // 50C7BF es un OUI real de TP-Link
    [InlineData("50c7bf112233", "TP-Link")]
    [InlineData("50:C7:BF:11:22:33", "TP-Link")]
    public void ResolvesKnownVendor(string mac, string expected) =>
        Assert.Equal(expected, _lookup.ResolveVendor(mac));

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("FF-FF-FF-00-00-00")]
    public void ReturnsNullForUnknown(string? mac) =>
        Assert.Null(_lookup.ResolveVendor(mac));
}
