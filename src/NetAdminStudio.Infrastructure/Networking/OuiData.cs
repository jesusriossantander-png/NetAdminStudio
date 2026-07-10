namespace NetAdminStudio.Infrastructure.Networking;

/// <summary>
/// Semilla local de prefijos OUI (los primeros 3 octetos de una MAC) → fabricante.
/// Datos embebidos para no depender de Internet. Se puede ampliar con el tiempo.
/// </summary>
internal static class OuiData
{
    public static readonly IReadOnlyDictionary<string, string> Map =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["50C7BF"] = "TP-Link",
            ["1C61B4"] = "TP-Link",
            ["001B63"] = "Apple",
            ["3C0754"] = "Apple",
            ["001A2B"] = "Cisco",
            ["00D0BC"] = "Cisco",
            ["DCA632"] = "Raspberry Pi",
            ["B827EB"] = "Raspberry Pi",
            ["E45F01"] = "Raspberry Pi",
            ["24A43C"] = "Ubiquiti",
            ["44D9E7"] = "Ubiquiti",
            ["744401"] = "Netgear",
            ["001E2A"] = "Netgear",
            ["001321"] = "Dell",
            ["B083FE"] = "Dell",
            ["3417EB"] = "Dell",
            ["00219B"] = "Dell",
            ["001560"] = "HP",
            ["A0481C"] = "HP",
            ["001279"] = "HP",
            ["8C1645"] = "Brother",
            ["008077"] = "Brother",
            ["0000AA"] = "Xerox",
            ["0004E2"] = "SMC",
            ["001CC0"] = "Intel",
        };
}
