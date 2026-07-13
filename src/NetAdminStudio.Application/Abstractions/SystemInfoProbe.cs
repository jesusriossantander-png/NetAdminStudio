namespace NetAdminStudio.Application.Abstractions;

/// <summary>Información de un disco lógico.</summary>
public sealed record DiskInfo(string Drive, long TotalBytes, long FreeBytes)
{
    public long UsedBytes => TotalBytes - FreeBytes;
    public int UsagePercent => TotalBytes == 0 ? 0 : (int)Math.Round(UsedBytes * 100.0 / TotalBytes);
    public double TotalGb => Math.Round(TotalBytes / 1073741824.0, 1);
    public double FreeGb => Math.Round(FreeBytes / 1073741824.0, 1);
}

/// <summary>Instantánea del estado del equipo local (SO, CPU, RAM, discos, servicios).</summary>
public sealed record SystemInfo(
    string MachineName,
    string OperatingSystem,
    string? CpuModel,
    int CpuCores,
    int LogicalProcessors,
    int? CpuUsagePercent,
    long TotalMemoryBytes,
    long AvailableMemoryBytes,
    int ServicesRunning,
    int ServicesTotal,
    IReadOnlyList<DiskInfo> Disks)
{
    public long UsedMemoryBytes => TotalMemoryBytes - AvailableMemoryBytes;

    public int MemoryUsagePercent => TotalMemoryBytes == 0
        ? 0
        : (int)Math.Round(UsedMemoryBytes * 100.0 / TotalMemoryBytes);

    public double TotalMemoryGb => Math.Round(TotalMemoryBytes / 1073741824.0, 1);
    public double UsedMemoryGb => Math.Round(UsedMemoryBytes / 1073741824.0, 1);
}

/// <summary>Sondeo del estado del equipo donde corre la aplicación.</summary>
public interface ISystemInfoProbe
{
    Task<SystemInfo> GetLocalSystemInfoAsync(CancellationToken ct);
}

/// <summary>Sondeo del estado de un equipo remoto vía WMI (requiere credenciales).</summary>
public interface IRemoteSystemInfoProbe
{
    Task<SystemInfo> GetAsync(string host, string username, string password, CancellationToken ct);
}
