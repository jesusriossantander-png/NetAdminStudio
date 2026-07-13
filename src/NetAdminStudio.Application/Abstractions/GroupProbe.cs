namespace NetAdminStudio.Application.Abstractions;

/// <summary>Un grupo local del equipo.</summary>
public sealed record LocalGroup(string Name, string? Description);

/// <summary>Sondeo de los grupos locales del equipo. Solo observación.</summary>
public interface IGroupProbe
{
    Task<IReadOnlyList<LocalGroup>> GetLocalGroupsAsync(CancellationToken ct);
}
