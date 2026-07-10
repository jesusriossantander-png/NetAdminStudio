# Métricas y alertas de red

## Métricas iniciales

- latency_ms
- packet_loss_percent
- availability
- interface_rx_bps
- interface_tx_bps
- interface_errors
- interface_discards
- poe_power_watts
- uptime_seconds
- cpu_percent
- memory_percent
- temperature_celsius

## Política de almacenamiento

- datos recientes con granularidad completa;
- agregación horaria y diaria;
- retención configurable;
- limpieza segura mediante job;
- nunca borrar eventos o auditoría por una limpieza de métricas.

## Alertas

- dispositivo offline;
- latencia alta sostenida;
- pérdida de paquetes;
- puerto flapping;
- errores de interfaz;
- cambio inesperado de IP o MAC;
- firmware no identificado;
- SNMP no disponible;
- PoE cercano al presupuesto;
- topología modificada.

## Antirruido

- debounce;
- ventana de confirmación;
- recuperación automática;
- deduplicación;
- severidad;
- mantenimiento programado;
- reconocimiento manual.
