# Arquitectura de Auditoría

## Componentes

- AuditEventWriter
- AuditEventStore
- AuditQueryService
- CorrelationResolver
- EvidenceStore
- ReportEngine
- ReportTemplateEngine
- ReportScheduler
- ComplianceRuleEngine
- RetentionManager
- IntegrityVerifier

## Flujo de auditoría

1. Una acción genera un evento.
2. El evento se envuelve con metadata común.
3. Se valida esquema.
4. Se persiste en almacenamiento inmutable.
5. Se relaciona con correlationId y causationId.
6. Se indexa.
7. Se expone a consultas.
8. Puede alimentar alertas y reportes.

## Flujo de reportes

1. Se selecciona plantilla.
2. Se validan permisos.
3. Se ejecuta consulta reproducible.
4. Se genera snapshot de datos.
5. Se renderiza.
6. Se calcula hash.
7. Se registra auditoría.
8. Se entrega o programa.
