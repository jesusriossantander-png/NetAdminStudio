using NetAdminStudio.Domain.Assets;
using NetAdminStudio.Domain.Common;

namespace NetAdminStudio.Domain.Printers;

public sealed class PrinterDevice : Entity
{
    public required string Name { get; set; }
    public required string ConnectionType { get; set; }
    public string? IpAddress { get; set; }
    public string? HostName { get; set; }
    public string? QueueName { get; set; }
    public string? Model { get; set; }
    public OperationalState State { get; set; }
    public int? BlackLevelPercent { get; set; }
    public int PendingJobs { get; set; }
    public DateTimeOffset? LastSeenAt { get; set; }
}
