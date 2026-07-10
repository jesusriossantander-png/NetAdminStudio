CREATE TABLE IF NOT EXISTS identity_accounts (
    id TEXT PRIMARY KEY,
    source_type TEXT NOT NULL,
    source_host_id TEXT,
    domain_name TEXT,
    sid TEXT NOT NULL,
    account_name TEXT NOT NULL,
    display_name TEXT,
    account_type TEXT NOT NULL,
    enabled INTEGER,
    locked INTEGER,
    password_expired INTEGER,
    password_never_expires INTEGER,
    last_logon_at TEXT,
    account_expires_at TEXT,
    distinguished_name TEXT,
    department TEXT,
    title TEXT,
    email TEXT,
    manager_sid TEXT,
    first_seen_at TEXT,
    last_seen_at TEXT,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_identity_accounts_source_sid
ON identity_accounts(source_type, source_host_id, domain_name, sid);

CREATE TABLE IF NOT EXISTS identity_groups (
    id TEXT PRIMARY KEY,
    source_type TEXT NOT NULL,
    source_host_id TEXT,
    domain_name TEXT,
    sid TEXT NOT NULL,
    group_name TEXT NOT NULL,
    description TEXT,
    scope TEXT,
    category TEXT,
    is_privileged INTEGER NOT NULL DEFAULT 0,
    first_seen_at TEXT,
    last_seen_at TEXT
);

CREATE TABLE IF NOT EXISTS identity_memberships (
    id TEXT PRIMARY KEY,
    group_id TEXT NOT NULL,
    member_sid TEXT NOT NULL,
    member_type TEXT NOT NULL,
    is_direct INTEGER NOT NULL DEFAULT 1,
    observed_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS shared_resources (
    id TEXT PRIMARY KEY,
    host_asset_id TEXT NOT NULL,
    share_name TEXT NOT NULL,
    local_path TEXT,
    unc_path TEXT NOT NULL,
    description TEXT,
    share_type TEXT,
    state TEXT,
    total_bytes INTEGER,
    free_bytes INTEGER,
    first_seen_at TEXT,
    last_seen_at TEXT,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_shared_resources_host_name
ON shared_resources(host_asset_id, share_name);

CREATE TABLE IF NOT EXISTS permission_snapshots (
    id TEXT PRIMARY KEY,
    resource_id TEXT NOT NULL,
    path TEXT NOT NULL,
    snapshot_type TEXT NOT NULL,
    owner_sid TEXT,
    inheritance_enabled INTEGER,
    observed_at TEXT NOT NULL,
    collector_version TEXT,
    hash TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS permission_entries (
    id TEXT PRIMARY KEY,
    snapshot_id TEXT NOT NULL,
    principal_sid TEXT NOT NULL,
    principal_name TEXT,
    principal_type TEXT,
    rights TEXT NOT NULL,
    access_type TEXT NOT NULL,
    is_inherited INTEGER NOT NULL,
    inheritance_flags TEXT,
    propagation_flags TEXT,
    applies_to TEXT,
    raw_sddl TEXT
);

CREATE INDEX IF NOT EXISTS ix_permission_entries_snapshot
ON permission_entries(snapshot_id);

CREATE INDEX IF NOT EXISTS ix_permission_entries_principal
ON permission_entries(principal_sid);

CREATE TABLE IF NOT EXISTS effective_access_results (
    id TEXT PRIMARY KEY,
    resource_id TEXT NOT NULL,
    path TEXT NOT NULL,
    principal_sid TEXT NOT NULL,
    effective_rights TEXT,
    denied_rights TEXT,
    confidence TEXT NOT NULL,
    explanation_json TEXT,
    calculated_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS permission_change_plans (
    id TEXT PRIMARY KEY,
    requested_by TEXT NOT NULL,
    resource_id TEXT NOT NULL,
    target_path TEXT NOT NULL,
    operation_type TEXT NOT NULL,
    request_json TEXT NOT NULL,
    preview_json TEXT,
    backup_snapshot_id TEXT,
    state TEXT NOT NULL,
    created_at TEXT NOT NULL,
    approved_at TEXT,
    executed_at TEXT,
    completed_at TEXT
);

CREATE TABLE IF NOT EXISTS smb_sessions (
    id TEXT PRIMARY KEY,
    resource_id TEXT,
    client_name TEXT,
    client_ip TEXT,
    user_name TEXT,
    num_opens INTEGER,
    connected_at TEXT,
    observed_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS smb_open_files (
    id TEXT PRIMARY KEY,
    resource_id TEXT,
    session_id TEXT,
    relative_path_protected TEXT,
    user_name TEXT,
    lock_count INTEGER,
    observed_at TEXT NOT NULL
);
