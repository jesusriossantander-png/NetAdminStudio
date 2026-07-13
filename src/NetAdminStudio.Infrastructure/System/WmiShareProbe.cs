using System.Management;
using System.Runtime.Versioning;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.System;

/// <summary>Lee los recursos compartidos del equipo vía WMI (<c>Win32_Share</c>).</summary>
[SupportedOSPlatform("windows")]
public sealed class WmiShareProbe : IShareProbe
{
    public Task<IReadOnlyList<SharedFolder>> GetLocalSharesAsync(CancellationToken ct)
    {
        if (!OperatingSystem.IsWindows())
            return Task.FromResult<IReadOnlyList<SharedFolder>>(Array.Empty<SharedFolder>());

        return Task.Run<IReadOnlyList<SharedFolder>>(() =>
        {
            var shares = new List<SharedFolder>();
            using var searcher = new ManagementObjectSearcher(
                "SELECT Name, Path, Description, Type FROM Win32_Share");

            foreach (ManagementObject share in searcher.Get())
            {
                ct.ThrowIfCancellationRequested();
                var name = share["Name"]?.ToString() ?? "(sin nombre)";
                var path = share["Path"]?.ToString();
                var description = share["Description"]?.ToString();
                var type = Convert.ToUInt32(share["Type"] ?? 0u);

                shares.Add(new SharedFolder(
                    name,
                    string.IsNullOrWhiteSpace(path) ? null : path,
                    string.IsNullOrWhiteSpace(description) ? null : description,
                    DescribeType(type)));
            }

            return shares;
        }, ct);
    }

    // Win32_Share.Type: 0=Disco, 1=Cola de impresión, 2=Dispositivo, 3=IPC.
    // El bit 0x80000000 marca recursos administrativos (C$, ADMIN$...).
    private static string DescribeType(uint type)
    {
        var admin = (type & 0x80000000) != 0;
        var baseType = (type & 0x0FFFFFFF) switch
        {
            0 => "Carpeta",
            1 => "Impresora",
            2 => "Dispositivo",
            3 => "IPC",
            _ => "Otro"
        };
        return admin ? $"{baseType} (administrativo)" : baseType;
    }
}
