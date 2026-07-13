namespace NetAdminStudio.Application.Abstractions;

/// <summary>Una credencial guardada (sin exponer la contraseña).</summary>
public sealed record SavedCredential(string Host, string Username);

/// <summary>
/// Almacén seguro de credenciales para consultar equipos remotos. Las contraseñas se
/// guardan cifradas localmente y nunca se exponen en texto plano fuera del equipo.
/// El host "*" representa la credencial por defecto (se usa si no hay una específica).
/// </summary>
public interface ICredentialStore
{
    Task SaveAsync(string host, string username, string password, CancellationToken ct);

    /// <summary>Devuelve usuario y contraseña para un host (o la credencial por defecto).</summary>
    Task<(string Username, string Password)?> GetAsync(string host, CancellationToken ct);

    Task<IReadOnlyList<SavedCredential>> ListAsync(CancellationToken ct);

    Task DeleteAsync(string host, CancellationToken ct);
}
