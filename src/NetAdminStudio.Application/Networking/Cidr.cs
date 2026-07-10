using System.Net;
using System.Net.Sockets;

namespace NetAdminStudio.Application.Networking;

/// <summary>
/// Utilidades para calcular las direcciones host de un rango CIDR IPv4.
/// </summary>
public static class Cidr
{
    /// <summary>Prefijo mínimo permitido (/22 = 1024 direcciones). Límite duro de seguridad.</summary>
    public const int MinPrefix = 22;

    /// <summary>
    /// Devuelve las direcciones host "usables" del rango (excluye red y broadcast).
    /// Lanza <see cref="ArgumentException"/> si el CIDR es inválido o el rango es demasiado grande.
    /// </summary>
    public static IReadOnlyList<IPAddress> Hosts(string cidr)
    {
        if (string.IsNullOrWhiteSpace(cidr))
            throw new ArgumentException("CIDR vacío.", nameof(cidr));

        var parts = cidr.Split('/');
        if (parts.Length != 2
            || !IPAddress.TryParse(parts[0], out var baseIp)
            || baseIp.AddressFamily != AddressFamily.InterNetwork
            || !int.TryParse(parts[1], out var prefix)
            || prefix is < 0 or > 32)
            throw new ArgumentException($"CIDR inválido: {cidr}", nameof(cidr));

        if (prefix < MinPrefix)
            throw new ArgumentException(
                $"Rango demasiado grande (prefijo /{prefix}); mínimo permitido /{MinPrefix}.",
                nameof(cidr));

        var baseBytes = baseIp.GetAddressBytes();
        var baseValue = (uint)((baseBytes[0] << 24) | (baseBytes[1] << 16)
                             | (baseBytes[2] << 8) | baseBytes[3]);
        var mask = prefix == 0 ? 0u : uint.MaxValue << (32 - prefix);
        var network = baseValue & mask;
        var broadcast = network | ~mask;

        var hosts = new List<IPAddress>();
        // /31 y /32 no tienen hosts "usables" en este modelo; devuelven lista vacía.
        for (var value = network + 1; value < broadcast; value++)
        {
            var bytes = new[]
            {
                (byte)(value >> 24), (byte)(value >> 16),
                (byte)(value >> 8), (byte)value
            };
            hosts.Add(new IPAddress(bytes));
        }
        return hosts;
    }
}
