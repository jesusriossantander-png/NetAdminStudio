# Seguridad

## Permisos RBAC

- Identities.View
- Identities.ViewSensitive
- Identities.Refresh
- Groups.View
- Shares.View
- Shares.ViewSessions
- Shares.ViewOpenFiles
- Permissions.View
- Permissions.CalculateEffective
- Permissions.CreatePlan
- Permissions.ExecutePlan
- Permissions.Rollback
- Permissions.ChangeOwner

## Controles

- agentes con cuenta de servicio dedicada;
- JEA/PowerShell restringido cuando se use;
- LDAP seguro;
- secretos mediante DPAPI;
- allowlist de hosts;
- validación de rutas;
- protección contra path traversal;
- no seguir reparse points;
- auditoría;
- doble confirmación para cambios masivos;
- bloqueo optimista por hash de snapshot.
