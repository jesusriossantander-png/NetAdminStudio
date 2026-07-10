# Eventos de dominio

## Red

- NetworkAssetDiscovered
- NetworkAssetUpdated
- NetworkAssetStateChanged
- NetworkAddressChanged
- NetworkPortStateChanged
- NetworkLinkDiscovered
- NetworkLinkChanged
- NetworkScanStarted
- NetworkScanCompleted
- NetworkAlertOpened
- NetworkAlertResolved

## Impresoras

- PrinterDiscovered
- PrinterStateChanged
- PrinterQueueChanged
- PrinterConsumableLow
- PrinterCounterRecorded
- PrinterMaintenanceCreated
- PrinterMaintenanceCompleted
- PrinterAlertOpened
- PrinterAlertResolved

## Envelope

Todos los eventos usan el envelope definido en Pack 2B:

- messageId
- messageType
- schemaVersion
- occurredAt
- producer
- correlationId
- causationId
- payload
