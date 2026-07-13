using System.Management;
using System.Runtime.Versioning;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.System;

/// <summary>
/// Lectura de <see cref="SystemInfo"/> desde WMI, sirve tanto para el equipo local como
/// para uno remoto: la diferencia es el <see cref="ManagementScope"/> que se le pasa.
/// </summary>
[SupportedOSPlatform("windows")]
internal static class WmiSystemReader
{
    public static SystemInfo Read(ManagementScope scope, string machineName, CancellationToken ct)
    {
        scope.Connect();

        var (osName, totalMem, availMem) = ReadOperatingSystem(scope);
        var (cpuModel, cores, logical, load) = ReadProcessor(scope);
        var disks = ReadDisks(scope, ct);
        var (running, total) = ReadServices(scope);

        return new SystemInfo(
            machineName, osName, cpuModel, cores, logical, load,
            totalMem, availMem, running, total, disks);
    }

    private static ManagementObjectSearcher Query(ManagementScope scope, string sql) =>
        new(scope, new ObjectQuery(sql));

    private static (string osName, long totalBytes, long availBytes) ReadOperatingSystem(ManagementScope scope)
    {
        using var searcher = Query(scope,
            "SELECT Caption, TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
        foreach (ManagementObject os in searcher.Get())
        {
            var caption = os["Caption"]?.ToString()?.Trim() ?? "Windows";
            var totalKb = ToLong(os["TotalVisibleMemorySize"]);
            var freeKb = ToLong(os["FreePhysicalMemory"]);
            return (caption, totalKb * 1024, freeKb * 1024);
        }
        return ("Windows", 0, 0);
    }

    private static (string? model, int cores, int logical, int? load) ReadProcessor(ManagementScope scope)
    {
        using var searcher = Query(scope,
            "SELECT Name, NumberOfCores, NumberOfLogicalProcessors, LoadPercentage FROM Win32_Processor");
        foreach (ManagementObject cpu in searcher.Get())
        {
            var model = cpu["Name"]?.ToString()?.Trim();
            var cores = (int)ToLong(cpu["NumberOfCores"]);
            var logical = (int)ToLong(cpu["NumberOfLogicalProcessors"]);
            var loadObj = cpu["LoadPercentage"];
            int? load = loadObj is null ? null : (int)ToLong(loadObj);
            return (model, cores, logical, load);
        }
        return (null, 0, 0, null);
    }

    private static IReadOnlyList<DiskInfo> ReadDisks(ManagementScope scope, CancellationToken ct)
    {
        var disks = new List<DiskInfo>();
        using var searcher = Query(scope,
            "SELECT DeviceID, Size, FreeSpace FROM Win32_LogicalDisk WHERE DriveType = 3");
        foreach (ManagementObject disk in searcher.Get())
        {
            ct.ThrowIfCancellationRequested();
            var drive = disk["DeviceID"]?.ToString() ?? "?";
            var size = ToLong(disk["Size"]);
            var free = ToLong(disk["FreeSpace"]);
            if (size > 0)
                disks.Add(new DiskInfo(drive, size, free));
        }
        return disks;
    }

    private static (int running, int total) ReadServices(ManagementScope scope)
    {
        var running = 0;
        var total = 0;
        using var searcher = Query(scope, "SELECT State FROM Win32_Service");
        foreach (ManagementObject svc in searcher.Get())
        {
            total++;
            if (string.Equals(svc["State"]?.ToString(), "Running", StringComparison.OrdinalIgnoreCase))
                running++;
        }
        return (running, total);
    }

    private static long ToLong(object? value) => value is null ? 0 : Convert.ToInt64(value);
}
