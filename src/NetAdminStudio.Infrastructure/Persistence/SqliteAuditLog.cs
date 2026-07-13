using NetAdminStudio.Application.Abstractions;

namespace NetAdminStudio.Infrastructure.Persistence;

/// <summary>Registro de auditoría persistido en SQLite.</summary>
public sealed class SqliteAuditLog(Database database) : IAuditLog
{
    public async Task LogAsync(string action, string detail, CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText =
            "INSERT INTO audit_log(id, at, action, detail) VALUES($id, $at, $action, $detail)";
        cmd.Parameters.AddWithValue("$id", Guid.NewGuid().ToString());
        cmd.Parameters.AddWithValue("$at", DateTimeOffset.UtcNow.ToString("O"));
        cmd.Parameters.AddWithValue("$action", action);
        cmd.Parameters.AddWithValue("$detail", detail);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IReadOnlyList<AuditEntry>> GetRecentAsync(int limit, CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT id, at, action, detail FROM audit_log ORDER BY at DESC LIMIT $limit";
        cmd.Parameters.AddWithValue("$limit", limit);
        using var reader = await cmd.ExecuteReaderAsync(ct);

        var items = new List<AuditEntry>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AuditEntry(
                Guid.Parse(reader.GetString(0)),
                DateTimeOffset.Parse(reader.GetString(1)),
                reader.GetString(2),
                reader.GetString(3)));
        }
        return items;
    }
}
