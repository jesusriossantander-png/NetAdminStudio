using System.Management;
using System.Runtime.Versioning;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.System;

/// <summary>Estado del equipo local (SO, CPU, memoria, discos, servicios) vía WMI.</summary>
[SupportedOSPlatform("windows")]
public sealed class WmiSystemInfoProbe : ISystemInfoProbe
{
    public Task<SystemInfo> GetLocalSystemInfoAsync(CancellationToken ct)
    {
        if (!OperatingSystem.IsWindows())
            return Task.FromResult(new SystemInfo(
                Environment.MachineName, "No disponible", null, 0, 0, null, 0, 0, 0, 0,
                Array.Empty<DiskInfo>()));

        return Task.Run(() =>
        {
            var scope = new ManagementScope(@"\\.\root\cimv2");
            return WmiSystemReader.Read(scope, Environment.MachineName, ct);
        }, ct);
    }
}
