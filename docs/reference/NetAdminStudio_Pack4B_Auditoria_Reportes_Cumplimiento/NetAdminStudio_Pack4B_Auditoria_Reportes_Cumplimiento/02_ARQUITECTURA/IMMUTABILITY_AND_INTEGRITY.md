# Inmutabilidad e integridad

## Decisión

Los eventos de auditoría no se actualizan ni eliminan desde la aplicación normal.

## Controles

- append-only;
- hash por evento;
- cadena de hash opcional por lote;
- hash del payload;
- batch seal;
- verificación periódica;
- backups;
- exportación con manifiesto;
- separación de permisos.

## Verificación

Cada lote puede incluir:

- BatchId
- FirstEventId
- LastEventId
- EventCount
- PreviousBatchHash
- BatchHash
- SealedAt

## Limitaciones MVP

SQLite no ofrece WORM real. La integridad se mejora mediante hashing, backups, permisos NTFS y verificación externa.
