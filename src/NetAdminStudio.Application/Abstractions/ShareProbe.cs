namespace NetAdminStudio.Application.Abstractions;

/// <summary>Una carpeta o recurso compartido del equipo.</summary>
public sealed record SharedFolder(
    string Name,
    string? Path,
    string? Description,
    string ShareType);

/// <summary>Sondeo de los recursos compartidos (SMB) del equipo local. Solo observación.</summary>
public interface IShareProbe
{
    Task<IReadOnlyList<SharedFolder>> GetLocalSharesAsync(CancellationToken ct);
}
