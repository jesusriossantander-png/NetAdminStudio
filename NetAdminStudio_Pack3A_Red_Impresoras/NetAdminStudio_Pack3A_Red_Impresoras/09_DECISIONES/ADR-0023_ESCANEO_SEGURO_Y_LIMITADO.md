# ADR-0023 — Escaneo seguro y limitado

## Estado

Aceptado.

## Decisión

Todo escaneo exige rango permitido, exclusiones, concurrencia, timeout, rate limiting y auditoría.

## Rechazado

- escaneo ilimitado;
- escaneo externo;
- fuerza bruta;
- scripts de shell libres;
- detección invasiva por defecto.
