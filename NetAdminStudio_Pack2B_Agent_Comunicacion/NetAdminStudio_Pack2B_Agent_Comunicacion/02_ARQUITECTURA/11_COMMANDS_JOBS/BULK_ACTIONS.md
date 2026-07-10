# Acciones Masivas

## Modelo

La Console crea un BulkOperation.
Cada target genera un Job independiente.

## Estados globales

- Pending
- Running
- PartiallySucceeded
- Succeeded
- Failed
- Cancelled

## UI

Mostrar:
- total,
- completados,
- exitosos,
- fallidos,
- pendientes.

## Regla

Un fallo en una PC no cancela automáticamente las demás.
