-- NetAdminStudio Pack 3A - referencia de migración SQLite

CREATE TABLE IF NOT EXISTS network_assets (
    id TEXT PRIMARY KEY,
    asset_id TEXT NOT NULL UNIQUE,
    asset_type TEXT NOT NULL,
    display_name TEXT NOT NULL,
    vendor TEXT,
    model TEXT,
    serial_number TEXT,
    firmware_version TEXT,
    primary_ip_address TEXT,
    primary_mac_address TEXT,
    hostname TEXT,
    operational_state TEXT NOT NULL,
    source TEXT NOT NULL,
    confidence TEXT NOT NULL,
    first_seen_at TEXT,
    last_seen_at TEXT,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_network_assets_ip
ON network_assets(primary_ip_address);

CREATE INDEX IF NOT EXISTS ix_network_assets_mac
ON network_assets(primary_mac_address);

CREATE TABLE IF NOT EXISTS network_interfaces (
    id TEXT PRIMARY KEY,
    network_asset_id TEXT NOT NULL,
    name TEXT NOT NULL,
    description TEXT,
    mac_address TEXT,
    ip_addresses_json TEXT,
    operational_status TEXT,
    speed_mbps INTEGER,
    observed_at TEXT,
    FOREIGN KEY(network_asset_id) REFERENCES network_assets(id)
);

CREATE TABLE IF NOT EXISTS network_ports (
    id TEXT PRIMARY KEY,
    network_asset_id TEXT NOT NULL,
    port_index INTEGER NOT NULL,
    name TEXT,
    admin_status TEXT,
    operational_status TEXT,
    speed_mbps INTEGER,
    duplex TEXT,
    poe_state TEXT,
    poe_power_watts REAL,
    access_vlan_id INTEGER,
    native_vlan_id INTEGER,
    is_trunk INTEGER NOT NULL DEFAULT 0,
    error_count INTEGER,
    discard_count INTEGER,
    observed_at TEXT,
    FOREIGN KEY(network_asset_id) REFERENCES network_assets(id)
);

CREATE UNIQUE INDEX IF NOT EXISTS ux_network_ports_asset_index
ON network_ports(network_asset_id, port_index);

CREATE TABLE IF NOT EXISTS network_links (
    id TEXT PRIMARY KEY,
    source_asset_id TEXT NOT NULL,
    target_asset_id TEXT NOT NULL,
    source_interface_id TEXT,
    target_interface_id TEXT,
    link_type TEXT NOT NULL,
    status TEXT,
    evidence_type TEXT,
    confidence TEXT NOT NULL,
    is_manual INTEGER NOT NULL DEFAULT 0,
    first_seen_at TEXT,
    last_seen_at TEXT
);

CREATE TABLE IF NOT EXISTS network_metrics (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    network_asset_id TEXT NOT NULL,
    metric_name TEXT NOT NULL,
    metric_value REAL NOT NULL,
    unit TEXT,
    observed_at TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_network_metrics_asset_time
ON network_metrics(network_asset_id, observed_at);

CREATE TABLE IF NOT EXISTS network_alerts (
    id TEXT PRIMARY KEY,
    network_asset_id TEXT,
    alert_type TEXT NOT NULL,
    severity TEXT NOT NULL,
    state TEXT NOT NULL,
    title TEXT NOT NULL,
    details_json TEXT,
    opened_at TEXT NOT NULL,
    acknowledged_at TEXT,
    resolved_at TEXT
);

CREATE TABLE IF NOT EXISTS printers (
    id TEXT PRIMARY KEY,
    asset_id TEXT NOT NULL UNIQUE,
    display_name TEXT NOT NULL,
    vendor TEXT,
    model TEXT,
    serial_number TEXT,
    firmware_version TEXT,
    connection_type TEXT NOT NULL,
    host_asset_id TEXT,
    driver_name TEXT,
    operational_state TEXT NOT NULL,
    first_seen_at TEXT,
    last_seen_at TEXT,
    created_at TEXT NOT NULL,
    updated_at TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS printer_endpoints (
    id TEXT PRIMARY KEY,
    printer_id TEXT NOT NULL,
    protocol TEXT NOT NULL,
    address TEXT,
    port INTEGER,
    share_name TEXT,
    queue_name TEXT,
    device_uri TEXT,
    is_primary INTEGER NOT NULL DEFAULT 0,
    is_secure INTEGER NOT NULL DEFAULT 0,
    last_verified_at TEXT,
    FOREIGN KEY(printer_id) REFERENCES printers(id)
);

CREATE TABLE IF NOT EXISTS printer_queues (
    id TEXT PRIMARY KEY,
    printer_id TEXT NOT NULL,
    host_asset_id TEXT,
    name TEXT NOT NULL,
    share_name TEXT,
    driver_name TEXT,
    port_name TEXT,
    status TEXT,
    is_shared INTEGER NOT NULL DEFAULT 0,
    is_paused INTEGER NOT NULL DEFAULT 0,
    pending_jobs INTEGER,
    last_observed_at TEXT,
    FOREIGN KEY(printer_id) REFERENCES printers(id)
);

CREATE TABLE IF NOT EXISTS printer_consumables (
    id TEXT PRIMARY KEY,
    printer_id TEXT NOT NULL,
    consumable_type TEXT NOT NULL,
    slot TEXT,
    description TEXT,
    level_percent REAL,
    remaining_units REAL,
    estimated_pages INTEGER,
    part_number TEXT,
    observed_at TEXT NOT NULL,
    source TEXT NOT NULL,
    confidence TEXT NOT NULL,
    FOREIGN KEY(printer_id) REFERENCES printers(id)
);

CREATE TABLE IF NOT EXISTS printer_counters (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    printer_id TEXT NOT NULL,
    counter_name TEXT NOT NULL,
    counter_value INTEGER NOT NULL,
    observed_at TEXT NOT NULL,
    source TEXT NOT NULL,
    FOREIGN KEY(printer_id) REFERENCES printers(id)
);

CREATE INDEX IF NOT EXISTS ix_printer_counters_printer_time
ON printer_counters(printer_id, observed_at);

CREATE TABLE IF NOT EXISTS printer_maintenance (
    id TEXT PRIMARY KEY,
    printer_id TEXT NOT NULL,
    maintenance_type TEXT NOT NULL,
    description TEXT NOT NULL,
    technician TEXT,
    provider TEXT,
    started_at TEXT,
    completed_at TEXT,
    cost REAL,
    currency TEXT,
    details_json TEXT,
    created_by TEXT NOT NULL,
    created_at TEXT NOT NULL,
    FOREIGN KEY(printer_id) REFERENCES printers(id)
);

CREATE TABLE IF NOT EXISTS printer_alerts (
    id TEXT PRIMARY KEY,
    printer_id TEXT NOT NULL,
    alert_type TEXT NOT NULL,
    severity TEXT NOT NULL,
    state TEXT NOT NULL,
    title TEXT NOT NULL,
    details_json TEXT,
    opened_at TEXT NOT NULL,
    acknowledged_at TEXT,
    resolved_at TEXT,
    FOREIGN KEY(printer_id) REFERENCES printers(id)
);
