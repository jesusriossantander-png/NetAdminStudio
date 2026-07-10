# Modelo de evento de auditoría

## Campos mínimos

- EventId
- EventType
- Category
- Severity
- OccurredAt
- RecordedAt
- ActorType
- ActorId
- ActorDisplayName
- SourceType
- SourceId
- TargetType
- TargetId
- TargetDisplayName
- Action
- Result
- CorrelationId
- CausationId
- JobId
- AgentId
- SessionId
- IpAddress
- Hostname
- DataClassification
- PayloadJson
- PayloadHash
- SchemaVersion

## Categorías

- Authentication
- Authorization
- Inventory
- Network
- Printers
- Identity
- Permissions
- Configuration
- Jobs
- Reports
- Security
- System
- Maintenance

## Resultados

- Success
- Partial
- Failed
- Denied
- Cancelled
- TimedOut
- Unknown
