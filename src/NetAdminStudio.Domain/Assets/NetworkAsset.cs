using NetAdminStudio.Domain.Common;

namespace NetAdminStudio.Domain.Assets;

public enum AssetType
{
    Unknown, Router, Switch, AccessPoint, Firewall, Server,
    Workstation, RaspberryPi, Camera, Printer, Ups, Plc
}

public enum OperationalState
{
    Unknown, Online, Degraded, Offline, Maintenance, Retired
}

public sealed class NetworkAsset : Entity
{
    public required string Name { get; set; }
    public AssetType Type { get; set; }
    public OperationalState State { get; private set; }
    public string? IpAddress { get; set; }
    public string? MacAddress { get; set; }
    public string? Vendor { get; set; }
    public string? Model { get; set; }
    public string? Location { get; set; }
    public DateTimeOffset? LastSeenAt { get; private set; }
    public double? LatencyMs { get; private set; }

    public void RecordPresence(bool reachable, double? latencyMs, DateTimeOffset observedAt)
    {
        State = reachable ? OperationalState.Online : OperationalState.Offline;
        LatencyMs = latencyMs;
        LastSeenAt = observedAt;
    }

    public void MarkDegraded() => State = OperationalState.Degraded;
}
