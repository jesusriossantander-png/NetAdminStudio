using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.Networking;

/// <summary>Resuelve el fabricante a partir del prefijo OUI (primeros 3 octetos) de una MAC.</summary>
public sealed class OuiVendorLookup : IVendorLookup
{
    public string? ResolveVendor(string? macAddress)
    {
        if (string.IsNullOrWhiteSpace(macAddress))
            return null;

        var hex = new string(macAddress.Where(Uri.IsHexDigit).ToArray());
        if (hex.Length < 6)
            return null;

        var prefix = hex[..6].ToUpperInvariant();
        return OuiData.Map.TryGetValue(prefix, out var vendor) ? vendor : null;
    }
}
