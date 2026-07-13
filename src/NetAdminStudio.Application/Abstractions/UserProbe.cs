namespace NetAdminStudio.Application.Abstractions;

/// <summary>Una cuenta de usuario local del equipo.</summary>
public sealed record LocalUser(
    string Name,
    string? FullName,
    bool Disabled,
    string? Description);

/// <summary>Sondeo de las cuentas de usuario locales del equipo. Solo observación.</summary>
public interface IUserProbe
{
    Task<IReadOnlyList<LocalUser>> GetLocalUsersAsync(CancellationToken ct);
}
