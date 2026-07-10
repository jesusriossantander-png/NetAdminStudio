CREATE TABLE IF NOT EXISTS audit_events (
    id TEXT PRIMARY KEY,
    event_type TEXT NOT NULL,
    category TEXT NOT NULL,
    severity TEXT NOT NULL,
    occurred_at TEXT NOT NULL,
    recorded_at TEXT NOT NULL,
    actor_type TEXT,
    actor_id TEXT,
    actor_display_name TEXT,
    source_type TEXT,
    source_id TEXT,
    target_type TEXT,
    target_id TEXT,
    target_display_name TEXT,
    action TEXT NOT NULL,
    result TEXT NOT NULL,
    correlation_id TEXT,
    causation_id TEXT,
    job_id TEXT,
    agent_id TEXT,
    session_id TEXT,
    ip_address TEXT,
    hostname TEXT,
    data_classification TEXT,
    payload_json TEXT,
    payload_hash TEXT NOT NULL,
    schema_version TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_audit_events_occurred_at
ON audit_events(occurred_at);

CREATE INDEX IF NOT EXISTS ix_audit_events_actor
ON audit_events(actor_id);

CREATE INDEX IF NOT EXISTS ix_audit_events_target
ON audit_events(target_type, target_id);

CREATE INDEX IF NOT EXISTS ix_audit_events_correlation
ON audit_events(correlation_id);

CREATE TABLE IF NOT EXISTS audit_batches (
    id TEXT PRIMARY KEY,
    first_event_id TEXT NOT NULL,
    last_event_id TEXT NOT NULL,
    event_count INTEGER NOT NULL,
    previous_batch_hash TEXT,
    batch_hash TEXT NOT NULL,
    sealed_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS report_templates (
    id TEXT PRIMARY KEY,
    code TEXT NOT NULL UNIQUE,
    name TEXT NOT NULL,
    category TEXT NOT NULL,
    version TEXT NOT NULL,
    definition_json TEXT NOT NULL,
    enabled INTEGER NOT NULL DEFAULT 1,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS report_runs (
    id TEXT PRIMARY KEY,
    template_id TEXT NOT NULL,
    requested_by TEXT NOT NULL,
    state TEXT NOT NULL,
    parameters_json TEXT,
    dataset_hash TEXT,
    output_format TEXT NOT NULL,
    output_path_protected TEXT,
    output_hash TEXT,
    started_at TEXT,
    completed_at TEXT,
    error_json TEXT,
    created_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS report_schedules (
    id TEXT PRIMARY KEY,
    template_id TEXT NOT NULL,
    name TEXT NOT NULL,
    schedule_expression TEXT NOT NULL,
    timezone TEXT NOT NULL,
    parameters_json TEXT,
    delivery_json TEXT,
    enabled INTEGER NOT NULL DEFAULT 1,
    last_run_at TEXT,
    next_run_at TEXT,
    created_by TEXT NOT NULL,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS compliance_rules (
    id TEXT PRIMARY KEY,
    code TEXT NOT NULL UNIQUE,
    name TEXT NOT NULL,
    category TEXT NOT NULL,
    severity TEXT NOT NULL,
    version TEXT NOT NULL,
    definition_json TEXT NOT NULL,
    enabled INTEGER NOT NULL DEFAULT 1,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS compliance_evaluations (
    id TEXT PRIMARY KEY,
    rule_id TEXT NOT NULL,
    target_type TEXT NOT NULL,
    target_id TEXT NOT NULL,
    result TEXT NOT NULL,
    evidence_json TEXT,
    confidence TEXT NOT NULL,
    evaluated_at TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_compliance_target
ON compliance_evaluations(target_type, target_id);

CREATE TABLE IF NOT EXISTS evidence_items (
    id TEXT PRIMARY KEY,
    evidence_type TEXT NOT NULL,
    source_type TEXT,
    source_id TEXT,
    related_event_id TEXT,
    classification TEXT,
    content_path_protected TEXT,
    content_hash TEXT NOT NULL,
    metadata_json TEXT,
    created_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS retention_policies (
    id TEXT PRIMARY KEY,
    data_category TEXT NOT NULL UNIQUE,
    retention_days INTEGER,
    archive_after_days INTEGER,
    purge_enabled INTEGER NOT NULL DEFAULT 0,
    legal_hold INTEGER NOT NULL DEFAULT 0,
    updated_by TEXT NOT NULL,
    updated_at TEXT NOT NULL
);
