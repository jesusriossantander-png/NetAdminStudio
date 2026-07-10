# Persistencia — Red e Impresoras

## Tablas

### Red

- network_assets
- network_interfaces
- network_ports
- network_links
- network_observations
- network_metrics
- network_scans
- network_scan_targets
- network_alerts
- vlan_observations
- dhcp_observations
- dns_observations

### Impresoras

- printers
- printer_endpoints
- printer_queues
- print_job_observations
- printer_consumables
- printer_counters
- printer_maintenance
- printer_alerts

## Consideraciones SQLite

- usar claves TEXT con GUID;
- timestamps UTC ISO-8601;
- índices por asset_id, observed_at y state;
- WAL habilitado;
- migraciones versionadas;
- operaciones masivas en transacción;
- no almacenar secretos;
- limitar payloads crudos;
- separar series temporales de entidades maestras.
