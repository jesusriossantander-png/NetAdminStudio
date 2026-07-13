using System.Runtime.Versioning;
using System.Security.Cryptography;
using System.Text;
using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.Persistence;

/// <summary>
/// Almacén de credenciales en SQLite con la contraseña cifrada mediante DPAPI
/// (ámbito del usuario actual de Windows). El texto plano nunca se persiste.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class DpapiCredentialStore(Database database) : ICredentialStore
{
    private const string DefaultHost = "*";

    public async Task SaveAsync(string host, string username, string password, CancellationToken ct)
    {
        var key = string.IsNullOrWhiteSpace(host) ? DefaultHost : host.Trim();

        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = @"
INSERT INTO credentials(host, username, password_protected)
VALUES($host, $user, $pwd)
ON CONFLICT(host) DO UPDATE SET username=excluded.username, password_protected=excluded.password_protected;";
        cmd.Parameters.AddWithValue("$host", key);
        cmd.Parameters.AddWithValue("$user", username);
        cmd.Parameters.AddWithValue("$pwd", Protect(password));
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<(string Username, string Password)?> GetAsync(string host, CancellationToken ct)
    {
        return await ReadAsync(host, ct) ?? await ReadAsync(DefaultHost, ct);
    }

    private async Task<(string Username, string Password)?> ReadAsync(string host, CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT username, password_protected FROM credentials WHERE host=$host";
        cmd.Parameters.AddWithValue("$host", host);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct))
            return null;

        return (reader.GetString(0), Unprotect(reader.GetString(1)));
    }

    public async Task<IReadOnlyList<SavedCredential>> ListAsync(CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT host, username FROM credentials ORDER BY host";
        using var reader = await cmd.ExecuteReaderAsync(ct);

        var items = new List<SavedCredential>();
        while (await reader.ReadAsync(ct))
            items.Add(new SavedCredential(reader.GetString(0), reader.GetString(1)));
        return items;
    }

    public async Task DeleteAsync(string host, CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = "DELETE FROM credentials WHERE host=$host";
        cmd.Parameters.AddWithValue("$host", string.IsNullOrWhiteSpace(host) ? DefaultHost : host.Trim());
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static string Protect(string plain)
    {
        var bytes = ProtectedData.Protect(
            Encoding.UTF8.GetBytes(plain), null, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(bytes);
    }

    private static string Unprotect(string protectedBase64)
    {
        try
        {
            var bytes = ProtectedData.Unprotect(
                Convert.FromBase64String(protectedBase64), null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(bytes);
        }
        catch
        {
            return "";
        }
    }
}
