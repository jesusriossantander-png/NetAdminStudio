# Prompt — Implementar Pack 4A

Implementá Usuarios, Active Directory, Shares y Permisos respetando los Packs previos.

## Obligatorio

- WPF MVVM;
- Clean Architecture;
- SQLite;
- Result Pattern;
- Serilog;
- DPAPI;
- Agent + jobs;
- contratos versionados;
- auditoría;
- mínimo privilegio;
- adaptadores tipados;
- no usar shell libre;
- soporte de cancelación;
- tests.

## Capas

### Domain
Entidades, value objects, reglas de permisos, planes de cambio y eventos.

### Application
Queries, commands, handlers, validadores y políticas.

### Infrastructure
AD/LDAP, Local Accounts, SMB, NTFS, WMI y Windows APIs.

### Persistence
Migraciones, repositorios, snapshots e índices.

### UI
Usuarios, grupos, shares, explorador, acceso efectivo y vista previa.

## Requisito crítico

Nunca modificar ACL directamente desde la UI. La UI crea un plan; el agente valida y ejecuta.
