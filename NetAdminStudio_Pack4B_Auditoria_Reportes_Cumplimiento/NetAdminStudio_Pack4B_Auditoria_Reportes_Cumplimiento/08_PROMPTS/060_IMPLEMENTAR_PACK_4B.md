# Prompt — Implementar Pack 4B

Implementar Auditoría, Reportes, Historial y Cumplimiento respetando todos los packs anteriores.

## Obligatorio

- Clean Architecture;
- WPF MVVM;
- SQLite MVP;
- Result Pattern;
- Serilog;
- jobs;
- auditoría append-only;
- hashing;
- contratos versionados;
- RBAC;
- exportación PDF/Excel/CSV;
- programación;
- retención;
- tests.

## Capas

### Domain
AuditEvent, Evidence, ReportTemplate, ReportRun, ComplianceRule, ComplianceEvaluation, RetentionPolicy.

### Application
Commands, queries, handlers, schedulers, validators.

### Infrastructure
Renderers, exporters, mail delivery, file storage, hash verification.

### Persistence
Migraciones, índices, repositorios, append-only enforcement.

### UI
Audit Center, History, Report Center, Compliance Dashboard, Report Builder.

## Requisito crítico

No permitir modificaciones ni borrado directo de eventos de auditoría.
