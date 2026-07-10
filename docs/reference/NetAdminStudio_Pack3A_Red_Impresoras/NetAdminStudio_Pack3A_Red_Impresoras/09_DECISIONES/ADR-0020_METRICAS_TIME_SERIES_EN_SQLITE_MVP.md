# ADR-0020 — Series temporales en SQLite para MVP

## Estado

Aceptado con límites.

## Decisión

Usar SQLite con índices y retención para métricas del MVP. Diseñar repositorios para migrar posteriormente.

## Límites

- agregación;
- retención;
- inserciones por lotes;
- WAL;
- no usar para telemetría masiva indefinida.
