using NetAdminStudio.Domain.Assets;

namespace NetAdminStudio.Application.Networking;

/// <summary>
/// Estima el tipo de dispositivo combinando los puertos abiertos y el fabricante (OUI).
/// Lógica pura y testeable, sin acceso a red.
/// </summary>
public static class AssetClassifier
{
    private static readonly string[] NetworkVendors =
        { "tp-link", "cisco", "mikrotik", "ubiquiti", "netgear", "d-link", "huawei", "aruba" };

    public static AssetType Classify(IReadOnlyList<int> openPorts, string? vendor)
    {
        bool Has(int p) => openPorts.Contains(p);
        var v = vendor?.ToLowerInvariant() ?? "";
        var networkVendor = NetworkVendors.Any(nv => v.Contains(nv));

        if (Has(9100) || Has(515) || Has(631))
            return AssetType.Printer;

        if (Has(161) && networkVendor)
            return AssetType.Switch;

        if (Has(445) && Has(3389))
            return AssetType.Workstation;

        if (Has(445))
            return AssetType.Server;

        return AssetType.Unknown;
    }
}
