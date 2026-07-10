using NetAdminStudio.Domain.Common;

namespace NetAdminStudio.Domain.Alerts;

public enum AlertSeverity { Info, Warning, Critical }
public enum AlertState { Open, Acknowledged, Resolved }

public sealed class Alert : Entity
{
    public required string Title { get; set; }
    public required string SourceType { get; set; }
    public Guid SourceId { get; set; }
    public AlertSeverity Severity { get; set; }
    public AlertState State { get; private set; } = AlertState.Open;
    public DateTimeOffset OpenedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ResolvedAt { get; private set; }

    public void Acknowledge() => State = AlertState.Acknowledged;

    public void Resolve()
    {
        State = AlertState.Resolved;
        ResolvedAt = DateTimeOffset.UtcNow;
    }
}
