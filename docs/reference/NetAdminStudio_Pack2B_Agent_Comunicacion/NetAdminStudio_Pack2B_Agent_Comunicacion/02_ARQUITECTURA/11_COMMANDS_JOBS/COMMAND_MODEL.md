# Modelo de Comandos

## Command

Representa una intención de modificar o consultar el nodo.

## Propiedades

- CommandId.
- CommandType.
- TargetAgentId.
- RequestedBy.
- RequestedAt.
- Parameters.
- DryRun.
- RequiredPermission.
- IdempotencyKey.

## Estados

- Received
- Validated
- Rejected
- Queued
- Running
- Succeeded
- Failed
- Cancelled
- TimedOut
