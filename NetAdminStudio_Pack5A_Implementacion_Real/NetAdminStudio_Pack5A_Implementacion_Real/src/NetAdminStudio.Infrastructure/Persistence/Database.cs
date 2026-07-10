using Microsoft.Data.Sqlite;

namespace NetAdminStudio.Infrastructure.Persistence;

public sealed class Database(string connectionString)
{
    public SqliteConnection Open()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }

    public void Initialize()
    {
        using var db = Open();
        using var command = db.CreateCommand();
        command.CommandText = @"
PRAGMA journal_mode=WAL;

CREATE TABLE IF NOT EXISTS assets(
 id TEXT PRIMARY KEY,
 name TEXT NOT NULL,
 type INTEGER NOT NULL,
 state INTEGER NOT NULL,
 ip_address TEXT,
 mac_address TEXT,
 vendor TEXT,
 model TEXT,
 location TEXT,
 last_seen_at TEXT,
 latency_ms REAL
);

CREATE TABLE IF NOT EXISTS printers(
 id TEXT PRIMARY KEY,
 name TEXT NOT NULL,
 connection_type TEXT NOT NULL,
 ip_address TEXT,
 host_name TEXT,
 queue_name TEXT,
 model TEXT,
 state INTEGER NOT NULL,
 black_level_percent INTEGER,
 pending_jobs INTEGER NOT NULL,
 last_seen_at TEXT
);

CREATE TABLE IF NOT EXISTS alerts(
 id TEXT PRIMARY KEY,
 title TEXT NOT NULL,
 source_type TEXT NOT NULL,
 source_id TEXT NOT NULL,
 severity INTEGER NOT NULL,
 state INTEGER NOT NULL,
 opened_at TEXT NOT NULL,
 resolved_at TEXT
);

CREATE TABLE IF NOT EXISTS automation_rules(
 id TEXT PRIMARY KEY,
 name TEXT NOT NULL,
 trigger_type TEXT NOT NULL,
 condition_json TEXT NOT NULL,
 action_type TEXT NOT NULL,
 action_json TEXT NOT NULL,
 enabled INTEGER NOT NULL
);";
        command.ExecuteNonQuery();
    }
}
