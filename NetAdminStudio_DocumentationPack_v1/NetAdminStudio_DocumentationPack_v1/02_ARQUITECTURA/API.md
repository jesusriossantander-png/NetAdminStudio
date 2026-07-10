# API Interna

## API interna

La API interna define contratos entre Console, Agent y Core.

## Contratos base

- IAssetService
- IInventoryService
- IPrinterService
- IShareService
- IUserService
- IPermissionService
- IAuditService
- ICommandDispatcher
- IAgentClient

## Regla

La UI solo llama servicios. Nunca comandos directos.
