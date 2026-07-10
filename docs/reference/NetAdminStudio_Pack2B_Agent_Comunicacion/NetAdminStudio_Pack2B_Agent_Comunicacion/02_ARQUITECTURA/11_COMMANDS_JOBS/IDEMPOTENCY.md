# Idempotencia

## Objetivo

Evitar efectos duplicados por reintentos.

## Estrategia

Cada acción modificadora recibe `IdempotencyKey`.

El Agent guarda:
- clave,
- comando,
- resultado,
- vencimiento.

Si recibe la misma clave:
- no repite la acción,
- devuelve resultado previo.

## Ejemplos

Compartir una impresora dos veces no debe crear estados inconsistentes.
