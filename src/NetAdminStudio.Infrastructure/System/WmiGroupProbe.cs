using System.Management;
using System.Runtime.Versioning;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.System;

/// <summary>Lee los grupos locales vía WMI (<c>Win32_Group</c>).</summary>
[SupportedOSPlatform("windows")]
public sealed class WmiGroupProbe : IGroupProbe
{
    public Task<IReadOnlyList<LocalGroup>> GetLocalGroupsAsync(CancellationToken ct)
    {
        if (!OperatingSystem.IsWindows())
            return Task.FromResult<IReadOnlyList<LocalGroup>>(Array.Empty<LocalGroup>());

        return Task.Run<IReadOnlyList<LocalGroup>>(() =>
        {
            var groups = new List<LocalGroup>();
            using var searcher = new ManagementObjectSearcher(
                "SELECT Name, Description FROM Win32_Group WHERE LocalAccount = True");

            foreach (ManagementObject group in searcher.Get())
            {
                ct.ThrowIfCancellationRequested();
                var name = group["Name"]?.ToString() ?? "(sin nombre)";
                var description = group["Description"]?.ToString();
                groups.Add(new LocalGroup(
                    name,
                    string.IsNullOrWhiteSpace(description) ? null : description));
            }

            return groups;
        }, ct);
    }
}
