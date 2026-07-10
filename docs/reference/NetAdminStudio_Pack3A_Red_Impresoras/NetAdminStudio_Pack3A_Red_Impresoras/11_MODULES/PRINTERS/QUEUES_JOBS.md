# Colas y trabajos de impresión

## PrinterQueue

- Id
- PrinterId
- HostAssetId
- Name
- ShareName
- DriverName
- PortName
- Status
- IsShared
- IsPaused
- PendingJobs
- LastObservedAt

## PrintJobObservation

- ExternalJobId
- PrinterQueueId
- SubmittedBy
- DocumentNameProtected
- SubmittedAt
- StartedAt
- CompletedAt
- Pages
- Copies
- SizeBytes
- State
- ErrorCode
- Source

## Privacidad

El nombre del documento puede contener información sensible.

Políticas:

- ocultar por defecto;
- permitir hash o truncado;
- visibilidad por permiso;
- retención corta;
- auditoría de acceso;
- no capturar contenido del documento.
