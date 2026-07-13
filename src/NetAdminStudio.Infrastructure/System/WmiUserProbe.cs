using System.Management;
using System.Runtime.Versioning;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.System;

/// <summary>Lee las cuentas de usuario locales vía WMI (<c>Win32_UserAccount</c>).</summary>
[SupportedOSPlatform("windows")]
public sealed class WmiUserProbe : IUserProbe
{
    public Task<IReadOnlyList<LocalUser>> GetLocalUsersAsync(CancellationToken ct)
    {
        if (!OperatingSystem.IsWindows())
            return Task.FromResult<IReadOnlyList<LocalUser>>(Array.Empty<LocalUser>());

        return Task.Run<IReadOnlyList<LocalUser>>(() =>
        {
            var users = new List<LocalUser>();
            using var searcher = new ManagementObjectSearcher(
                "SELECT Name, FullName, Disabled, Description FROM Win32_UserAccount WHERE LocalAccount = True");

            foreach (ManagementObject user in searcher.Get())
            {
                ct.ThrowIfCancellationRequested();
                var name = user["Name"]?.ToString() ?? "(sin nombre)";
                var fullName = user["FullName"]?.ToString();
                var disabled = user["Disabled"] as bool? ?? false;
                var description = user["Description"]?.ToString();

                users.Add(new LocalUser(
                    name,
                    string.IsNullOrWhiteSpace(fullName) ? null : fullName,
                    disabled,
                    string.IsNullOrWhiteSpace(description) ? null : description));
            }

            return users;
        }, ct);
    }
}
