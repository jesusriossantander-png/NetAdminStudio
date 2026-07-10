# Arquitectura de Identidades

## Contextos

- Local Identity
- Active Directory Identity
- Authorization
- Shared Resources
- Permission Analysis
- Audit

## Componentes

- IdentityCollector
- GroupMembershipResolver
- ShareCollector
- NtfsAclReader
- SmbPermissionReader
- EffectiveAccessCalculator
- PermissionDiffEngine
- RemediationPlanner
- RemediationExecutor
- AuditWriter

## Flujo de lectura

1. El servidor crea un job.
2. El agente autorizado consulta el host.
3. Los adaptadores obtienen identidades, grupos, shares y ACL.
4. Los datos se normalizan.
5. Se calcula confianza y origen.
6. Se persiste snapshot.
7. Se generan hallazgos.
8. La UI muestra datos y diferencias.

## Flujo de cambio

1. Usuario solicita cambio.
2. Se valida permiso RBAC.
3. Se crea plan.
4. Se muestra vista previa.
5. Se genera respaldo de ACL.
6. Se ejecuta mediante agente.
7. Se verifica el resultado.
8. Se registra auditoría.
9. Ante fallo, se ofrece rollback.
