using System.Management;
using System.Runtime.Versioning;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.System;

/// <summary>
/// Lee el estado del equipo local vía WMI: sistema operativo, CPU, memoria, discos y servicios.
/// Solo observación, sin credenciales (equipo local).
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class WmiSystemInfoProbe : ISystemInfoProbe
{
    public Task<SystemInfo> GetLocalSystemInfoAsync(CancellationToken ct)
    {
        if (!OperatingSystem.IsWindows())
            return Task.FromResult(Empty());

        return Task.Run(() =>
        {
            var (osName, totalMem, availMem) = ReadOperatingSystem();
            var (cpuModel, cores, logical, load) = ReadProcessor();
            var disks = ReadDisks(ct);
            var (running, total) = ReadServices();

            return new SystemInfo(
                Environment.MachineName,
                osName,
                cpuModel,
                cores,
                logical,
                load,
                totalMem,
                availMem,
                running,
                total,
                disks);
        }, ct);
    }

    private static SystemInfo Empty() => new(
        Environment.MachineName, "No disponible", null, 0, 0, null, 0, 0, 0, 0,
        Array.Empty<DiskInfo>());

    private static (string osName, long totalBytes, long availBytes) ReadOperatingSystem()
    {
        using var searcher = new ManagementObjectSearcher(
            "SELECT Caption, TotalVisibleMemorySize, FreePhysicalMemory FROM Win32_OperatingSystem");
        foreach (ManagementObject os in searcher.Get())
        {
            var caption = os["Caption"]?.ToString()?.Trim() ?? "Windows";
            // Los valores WMI vienen en KiB.
            var totalKb = ToLong(os["TotalVisibleMemorySize"]);
            var freeKb = ToLong(os["FreePhysicalMemory"]);
            return (caption, totalKb * 1024, freeKb * 1024);
        }
        return ("Windows", 0, 0);
    }

    private static (string? model, int cores, int logical, int? load) ReadProcessor()
    {
        using var searcher = new ManagementObjectSearcher(
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

    private static IReadOnlyList<DiskInfo> ReadDisks(CancellationToken ct)
    {
        var disks = new List<DiskInfo>();
        using var searcher = new ManagementObjectSearcher(
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

    private static (int running, int total) ReadServices()
    {
        var running = 0;
        var total = 0;
        using var searcher = new ManagementObjectSearcher("SELECT State FROM Win32_Service");
        foreach (ManagementObject svc in searcher.Get())
        {
            total++;
            if (string.Equals(svc["State"]?.ToString(), "Running", StringComparison.OrdinalIgnoreCase))
                running++;
        }
        return (running, total);
    }

    private static long ToLong(object? value) =>
        value is null ? 0 : Convert.ToInt64(value);
}
