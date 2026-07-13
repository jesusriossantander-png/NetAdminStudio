using System.Management;
using System.Runtime.Versioning;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.System;

/// <summary>
/// Consulta el estado de un equipo remoto por WMI usando credenciales de administrador.
/// Requiere que el equipo destino tenga WMI/DCOM habilitado y el firewall lo permita.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class WmiRemoteSystemInfoProbe : IRemoteSystemInfoProbe
{
    public Task<SystemInfo> GetAsync(string host, string username, string password, CancellationToken ct)
    {
        if (!OperatingSystem.IsWindows())
            throw new PlatformNotSupportedException("WMI remoto solo está disponible en Windows.");

        return Task.Run(() =>
        {
            var options = new ConnectionOptions
            {
                Username = username,
                Password = password,
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.PacketPrivacy,
                EnablePrivileges = true,
                Timeout = TimeSpan.FromSeconds(20)
            };

            var scope = new ManagementScope($@"\\{host}\root\cimv2", options);
            return WmiSystemReader.Read(scope, host, ct);
        }, ct);
    }
}
