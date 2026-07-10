# Arquitectura de Datos

## Motor

SQLite local para MVP.

## Principio

Base orientada a historial. No guardar solo estado actual.

## Entidades principales

- Assets
- Agents
- NetworkInterfaces
- Printers
- Shares
- Users
- Groups
- Permissions
- Events
- Incidents
- Policies
- Reports
- AuditLogs
- Backups

## Migraciones

Usar EF Core Migrations o migrador propio versionado.
