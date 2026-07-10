using System.Net;

namespace NetAdminStudio.Application.Abstractions;

/// <summary>Resultado de sondear un host: alcanzable, nombre resuelto y latencia.</summary>
public sealed record HostProbe(bool Reachable, string? Hostname, double? LatencyMs);

/// <summary>Sondeo de hosts (ping + DNS inverso) y escaneo de puertos TCP.</summary>
public interface INetworkScanner
{
    Task<HostProbe> ProbeHostAsync(IPAddress ip, CancellationToken ct);

    Task<IReadOnlyList<int>> ScanPortsAsync(
        IPAddress ip, IReadOnlyList<int> ports, CancellationToken ct);
}

/// <summary>Lectura de la tabla ARP del sistema (mapa IP -> MAC).</summary>
public interface IArpTable
{
    /// <summary>Devuelve un mapa dirección IP (string) -> MAC normalizada "AA-BB-CC-11-22-33".</summary>
    Task<IReadOnlyDictionary<string, string>> GetAllAsync(CancellationToken ct);
}

/// <summary>Resolución de fabricante a partir del prefijo OUI de una MAC.</summary>
public interface IVendorLookup
{
    string? ResolveVendor(string? macAddress);
}

/// <summary>Lista fija y conservadora de puertos a escanear.</summary>
public static class ScanPorts
{
    public static readonly IReadOnlyList<int> Default =
        new[] { 22, 23, 53, 80, 161, 443, 445, 515, 631, 3389, 8080, 9100 };
}
