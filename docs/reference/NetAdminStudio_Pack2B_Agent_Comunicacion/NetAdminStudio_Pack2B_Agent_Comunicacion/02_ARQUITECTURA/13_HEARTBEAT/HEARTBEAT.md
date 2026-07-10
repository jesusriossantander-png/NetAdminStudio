# Heartbeat

## Objetivo

Informar presencia y salud mínima.

## Payload

- AgentId.
- TimestampUtc.
- AgentVersion.
- Status.
- CpuPercent.
- MemoryPercent.
- ActiveJobs.
- LastInventoryAt.
- Warnings.

## Frecuencia

30 segundos por defecto.

## Estado offline

Se considera offline después de 3 heartbeats perdidos.
Debe ser configurable.
