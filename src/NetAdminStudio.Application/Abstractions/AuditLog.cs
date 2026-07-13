namespace NetAdminStudio.Application.Abstractions;

/// <summary>Una entrada del registro de auditoría.</summary>
public sealed record AuditEntry(Guid Id, DateTimeOffset At, string Action, string Detail);

/// <summary>Registro persistente de acciones realizadas en el sistema.</summary>
public interface IAuditLog
{
    Task LogAsync(string action, string detail, CancellationToken ct);
    Task<IReadOnlyList<AuditEntry>> GetRecentAsync(int limit, CancellationToken ct);
}
