using NetAdminStudio.Infrastructure.Networking;

namespace NetAdminStudio.Tests;

public sealed class ArpTableParserTests
{
    private const string Sample = @"
Interfaz: 192.168.0.14 --- 0xb
  Dirección de Internet     Dirección física      Tipo
  192.168.0.1           50-c7-bf-11-22-33     dinámico
  192.168.0.50          8c-16-45-aa-bb-cc     dinámico
  192.168.0.255         ff-ff-ff-ff-ff-ff     estático
";

    [Fact]
    public void Parse_ExtractsIpToMacPairs()
    {
        var map = ArpTable.Parse(Sample);
        Assert.Equal("50-C7-BF-11-22-33", map["192.168.0.1"]);
        Assert.Equal("8C-16-45-AA-BB-CC", map["192.168.0.50"]);
    }

    [Fact]
    public void Parse_IgnoresBroadcastAndMulticast()
    {
        var map = ArpTable.Parse(Sample);
        Assert.False(map.ContainsKey("192.168.0.255"));
    }
}
