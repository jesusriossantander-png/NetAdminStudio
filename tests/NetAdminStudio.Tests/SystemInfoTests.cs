using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Tests;

public sealed class SystemInfoTests
{
    [Fact]
    public void Disk_ComputesUsageAndGb()
    {
        var disk = new DiskInfo("C:", TotalBytes: 100L * 1073741824, FreeBytes: 25L * 1073741824);
        Assert.Equal(75, disk.UsagePercent);
        Assert.Equal(100.0, disk.TotalGb);
        Assert.Equal(25.0, disk.FreeGb);
    }

    [Fact]
    public void Memory_ComputesUsagePercent()
    {
        var info = new SystemInfo(
            "PC-TEST", "Windows 11", "Intel i5", 4, 8, 30,
            TotalMemoryBytes: 16L * 1073741824,
            AvailableMemoryBytes: 4L * 1073741824,
            ServicesRunning: 100, ServicesTotal: 200,
            Disks: Array.Empty<DiskInfo>());

        Assert.Equal(75, info.MemoryUsagePercent);
        Assert.Equal(12.0, info.UsedMemoryGb);
        Assert.Equal(16.0, info.TotalMemoryGb);
    }

    [Fact]
    public void Disk_ZeroTotal_DoesNotDivideByZero()
    {
        var disk = new DiskInfo("Z:", 0, 0);
        Assert.Equal(0, disk.UsagePercent);
    }
}
