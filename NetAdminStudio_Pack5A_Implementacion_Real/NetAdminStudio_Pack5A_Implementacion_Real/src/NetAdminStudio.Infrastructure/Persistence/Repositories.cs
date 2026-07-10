using Microsoft.Data.Sqlite;
using NetAdminStudio.Application.Abstractions;
using NetAdminStudio.Domain.Alerts;
using NetAdminStudio.Domain.Assets;
using NetAdminStudio.Domain.Automation;
using NetAdminStudio.Domain.Printers;

namespace NetAdminStudio.Infrastructure.Persistence;

internal static class ReaderExtensions
{
    public static Guid GuidValue(this SqliteDataReader reader, string name) =>
        Guid.Parse(reader.GetString(reader.GetOrdinal(name)));

    public static string? NullableString(this SqliteDataReader reader, string name)
    {
        var index = reader.GetOrdinal(name);
        return reader.IsDBNull(index) ? null : reader.GetString(index);
    }
}

public sealed class AssetRepository(Database database) : IAssetRepository
{
    public async Task<IReadOnlyList<NetworkAsset>> GetAllAsync(CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT * FROM assets ORDER BY name";
        using var reader = await cmd.ExecuteReaderAsync(ct);

        var items = new List<NetworkAsset>();
        while (await reader.ReadAsync(ct))
        {
            var asset = new NetworkAsset
            {
                Id = reader.GuidValue("id"),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Type = (AssetType)reader.GetInt32(reader.GetOrdinal("type")),
                IpAddress = reader.NullableString("ip_address"),
                MacAddress = reader.NullableString("mac_address"),
                Vendor = reader.NullableString("vendor"),
                Model = reader.NullableString("model"),
                Location = reader.NullableString("location")
            };

            var state = (OperationalState)reader.GetInt32(reader.GetOrdinal("state"));
            if (state == OperationalState.Degraded)
            {
                asset.MarkDegraded();
            }
            else if (state is OperationalState.Online or OperationalState.Offline)
            {
                var latencyIndex = reader.GetOrdinal("latency_ms");
                double? latency = reader.IsDBNull(latencyIndex)
                    ? null
                    : reader.GetDouble(latencyIndex);

                var observedAt = DateTimeOffset.TryParse(
                    reader.NullableString("last_seen_at"), out var parsed)
                    ? parsed
                    : DateTimeOffset.UtcNow;

                asset.RecordPresence(state == OperationalState.Online, latency, observedAt);
            }

            items.Add(asset);
        }

        return items;
    }

    public async Task UpsertAsync(NetworkAsset asset, CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = @"
INSERT INTO assets(id,name,type,state,ip_address,mac_address,vendor,model,location,last_seen_at,latency_ms)
VALUES($id,$name,$type,$state,$ip,$mac,$vendor,$model,$location,$seen,$latency)
ON CONFLICT(id) DO UPDATE SET
 name=excluded.name,
 type=excluded.type,
 state=excluded.state,
 ip_address=excluded.ip_address,
 mac_address=excluded.mac_address,
 vendor=excluded.vendor,
 model=excluded.model,
 location=excluded.location,
 last_seen_at=excluded.last_seen_at,
 latency_ms=excluded.latency_ms;";

        cmd.Parameters.AddWithValue("$id", asset.Id.ToString());
        cmd.Parameters.AddWithValue("$name", asset.Name);
        cmd.Parameters.AddWithValue("$type", (int)asset.Type);
        cmd.Parameters.AddWithValue("$state", (int)asset.State);
        cmd.Parameters.AddWithValue("$ip", (object?)asset.IpAddress ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$mac", (object?)asset.MacAddress ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$vendor", (object?)asset.Vendor ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$model", (object?)asset.Model ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$location", (object?)asset.Location ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$seen", asset.LastSeenAt?.ToString("O") ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$latency", asset.LatencyMs ?? (object)DBNull.Value);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}

public sealed class PrinterRepository(Database database) : IPrinterRepository
{
    public async Task<IReadOnlyList<PrinterDevice>> GetAllAsync(CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT * FROM printers ORDER BY name";
        using var reader = await cmd.ExecuteReaderAsync(ct);

        var items = new List<PrinterDevice>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new PrinterDevice
            {
                Id = reader.GuidValue("id"),
                Name = reader.GetString(reader.GetOrdinal("name")),
                ConnectionType = reader.GetString(reader.GetOrdinal("connection_type")),
                IpAddress = reader.NullableString("ip_address"),
                HostName = reader.NullableString("host_name"),
                QueueName = reader.NullableString("queue_name"),
                Model = reader.NullableString("model"),
                State = (OperationalState)reader.GetInt32(reader.GetOrdinal("state")),
                BlackLevelPercent = reader.IsDBNull(reader.GetOrdinal("black_level_percent"))
                    ? null
                    : reader.GetInt32(reader.GetOrdinal("black_level_percent")),
                PendingJobs = reader.GetInt32(reader.GetOrdinal("pending_jobs")),
                LastSeenAt = DateTimeOffset.TryParse(reader.NullableString("last_seen_at"), out var value)
                    ? value
                    : null
            });
        }

        return items;
    }

    public async Task UpsertAsync(PrinterDevice printer, CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = @"
INSERT INTO printers(id,name,connection_type,ip_address,host_name,queue_name,model,state,black_level_percent,pending_jobs,last_seen_at)
VALUES($id,$name,$connection,$ip,$host,$queue,$model,$state,$level,$jobs,$seen)
ON CONFLICT(id) DO UPDATE SET
 name=excluded.name,
 connection_type=excluded.connection_type,
 ip_address=excluded.ip_address,
 host_name=excluded.host_name,
 queue_name=excluded.queue_name,
 model=excluded.model,
 state=excluded.state,
 black_level_percent=excluded.black_level_percent,
 pending_jobs=excluded.pending_jobs,
 last_seen_at=excluded.last_seen_at;";

        cmd.Parameters.AddWithValue("$id", printer.Id.ToString());
        cmd.Parameters.AddWithValue("$name", printer.Name);
        cmd.Parameters.AddWithValue("$connection", printer.ConnectionType);
        cmd.Parameters.AddWithValue("$ip", (object?)printer.IpAddress ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$host", (object?)printer.HostName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$queue", (object?)printer.QueueName ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$model", (object?)printer.Model ?? DBNull.Value);
        cmd.Parameters.AddWithValue("$state", (int)printer.State);
        cmd.Parameters.AddWithValue("$level", printer.BlackLevelPercent ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$jobs", printer.PendingJobs);
        cmd.Parameters.AddWithValue("$seen", printer.LastSeenAt?.ToString("O") ?? (object)DBNull.Value);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}

public sealed class AlertRepository(Database database) : IAlertRepository
{
    public async Task<IReadOnlyList<Alert>> GetOpenAsync(CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT * FROM alerts WHERE state <> 2 ORDER BY opened_at DESC";
        using var reader = await cmd.ExecuteReaderAsync(ct);

        var items = new List<Alert>();
        while (await reader.ReadAsync(ct))
        {
            var alert = new Alert
            {
                Id = reader.GuidValue("id"),
                Title = reader.GetString(reader.GetOrdinal("title")),
                SourceType = reader.GetString(reader.GetOrdinal("source_type")),
                SourceId = Guid.Parse(reader.GetString(reader.GetOrdinal("source_id"))),
                Severity = (AlertSeverity)reader.GetInt32(reader.GetOrdinal("severity")),
                OpenedAt = DateTimeOffset.Parse(reader.GetString(reader.GetOrdinal("opened_at")))
            };

            if ((AlertState)reader.GetInt32(reader.GetOrdinal("state")) == AlertState.Acknowledged)
                alert.Acknowledge();

            items.Add(alert);
        }

        return items;
    }

    public async Task<Alert?> GetAsync(Guid id, CancellationToken ct) =>
        (await GetOpenAsync(ct)).FirstOrDefault(x => x.Id == id);

    public async Task AddAsync(Alert alert, CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = @"
INSERT OR IGNORE INTO alerts(id,title,source_type,source_id,severity,state,opened_at,resolved_at)
VALUES($id,$title,$sourceType,$sourceId,$severity,$state,$opened,$resolved);";

        cmd.Parameters.AddWithValue("$id", alert.Id.ToString());
        cmd.Parameters.AddWithValue("$title", alert.Title);
        cmd.Parameters.AddWithValue("$sourceType", alert.SourceType);
        cmd.Parameters.AddWithValue("$sourceId", alert.SourceId.ToString());
        cmd.Parameters.AddWithValue("$severity", (int)alert.Severity);
        cmd.Parameters.AddWithValue("$state", (int)alert.State);
        cmd.Parameters.AddWithValue("$opened", alert.OpenedAt.ToString("O"));
        cmd.Parameters.AddWithValue("$resolved", alert.ResolvedAt?.ToString("O") ?? (object)DBNull.Value);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task SaveAsync(Alert alert, CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = "UPDATE alerts SET state=$state,resolved_at=$resolved WHERE id=$id";
        cmd.Parameters.AddWithValue("$state", (int)alert.State);
        cmd.Parameters.AddWithValue("$resolved", alert.ResolvedAt?.ToString("O") ?? (object)DBNull.Value);
        cmd.Parameters.AddWithValue("$id", alert.Id.ToString());
        await cmd.ExecuteNonQueryAsync(ct);
    }
}

public sealed class AutomationRepository(Database database) : IAutomationRepository
{
    public async Task<IReadOnlyList<AutomationRule>> GetAllAsync(CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = "SELECT * FROM automation_rules ORDER BY name";
        using var reader = await cmd.ExecuteReaderAsync(ct);

        var items = new List<AutomationRule>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AutomationRule
            {
                Id = reader.GuidValue("id"),
                Name = reader.GetString(reader.GetOrdinal("name")),
                TriggerType = reader.GetString(reader.GetOrdinal("trigger_type")),
                ConditionJson = reader.GetString(reader.GetOrdinal("condition_json")),
                ActionType = reader.GetString(reader.GetOrdinal("action_type")),
                ActionJson = reader.GetString(reader.GetOrdinal("action_json")),
                Enabled = reader.GetInt32(reader.GetOrdinal("enabled")) == 1
            });
        }

        return items;
    }

    public async Task AddAsync(AutomationRule rule, CancellationToken ct)
    {
        using var db = database.Open();
        using var cmd = db.CreateCommand();
        cmd.CommandText = @"
INSERT INTO automation_rules(id,name,trigger_type,condition_json,action_type,action_json,enabled)
VALUES($id,$name,$trigger,$condition,$action,$actionJson,$enabled);";

        cmd.Parameters.AddWithValue("$id", rule.Id.ToString());
        cmd.Parameters.AddWithValue("$name", rule.Name);
        cmd.Parameters.AddWithValue("$trigger", rule.TriggerType);
        cmd.Parameters.AddWithValue("$condition", rule.ConditionJson);
        cmd.Parameters.AddWithValue("$action", rule.ActionType);
        cmd.Parameters.AddWithValue("$actionJson", rule.ActionJson);
        cmd.Parameters.AddWithValue("$enabled", rule.Enabled ? 1 : 0);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
