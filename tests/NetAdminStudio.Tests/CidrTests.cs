using System.Net;
using NetAdminStudio.Application.Networking;

namespace NetAdminStudio.Tests;

public sealed class CidrTests
{
    [Fact]
    public void Hosts_Slash30_ReturnsTwoUsableHosts()
    {
        var hosts = Cidr.Hosts("192.168.0.0/30");
        Assert.Equal(2, hosts.Count);
        Assert.Equal(IPAddress.Parse("192.168.0.1"), hosts[0]);
        Assert.Equal(IPAddress.Parse("192.168.0.2"), hosts[1]);
    }

    [Fact]
    public void Hosts_Slash24_Returns254Hosts()
    {
        var hosts = Cidr.Hosts("10.0.0.0/24");
        Assert.Equal(254, hosts.Count);
        Assert.DoesNotContain(IPAddress.Parse("10.0.0.0"), hosts);   // red
        Assert.DoesNotContain(IPAddress.Parse("10.0.0.255"), hosts); // broadcast
    }

    [Theory]
    [InlineData("no-es-cidr")]
    [InlineData("192.168.0.0/21")]   // demasiado grande
    [InlineData("999.0.0.0/24")]
    public void Hosts_InvalidOrTooLarge_Throws(string cidr) =>
        Assert.Throws<ArgumentException>(() => Cidr.Hosts(cidr));
}
