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

    public string? Hostname { get; set; }
    public IReadOnlyList<int> OpenPorts { get; set; } = Array.Empty<int>();
    public DateTimeOffset? FirstSeenAt { get; set; }
    public string Origin { get; set; } = "demo";

    public void RecordPresence(bool reachable, double? latencyMs, DateTimeOffset observedAt)
    {
        State = reachable ? OperationalState.Online : OperationalState.Offline;
        LatencyMs = latencyMs;
        LastSeenAt = observedAt;
    }

    public void RecordDiscovery(
        string? hostname, string? mac, string? vendor,
        IReadOnlyList<int> openPorts, AssetType type,
        double? latencyMs, DateTimeOffset observedAt)
    {
        Hostname = hostname;
        if (!string.IsNullOrWhiteSpace(mac)) MacAddress = mac;
        if (!string.IsNullOrWhiteSpace(vendor)) Vendor = vendor;
        OpenPorts = openPorts;
        if (type != AssetType.Unknown) Type = type;
        Origin = "discovery";
        FirstSeenAt ??= observedAt;
        RecordPresence(true, latencyMs, observedAt);
    }

    public void MarkDegraded() => State = OperationalState.Degraded;
}
