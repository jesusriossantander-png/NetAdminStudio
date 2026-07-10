# Tablas Iniciales

## Assets

- Id
- AssetCode
- Name
- Type
- Status
- CurrentIp
- PrimaryMac
- FirstSeenAt
- LastSeenAt

## Agents

- Id
- AssetId
- Version
- Port
- Status
- LastHeartbeat

## Events

- Id
- AssetId
- EventType
- Severity
- Message
- OldValue
- NewValue
- CreatedAt
- CreatedBy

## AuditLogs

- Id
- Actor
- SourcePc
- TargetAssetId
- Action
- Result
- CreatedAt

## Printers

- Id
- AssetId
- Name
- Driver
- Port
- IsShared
- ShareName
- IsDefault

## Shares

- Id
- AssetId
- Name
- LocalPath
- Description
- IsHidden

## Permissions

- Id
- ResourceType
- ResourceId
- Principal
- PermissionType
- Rights
- Inherited
