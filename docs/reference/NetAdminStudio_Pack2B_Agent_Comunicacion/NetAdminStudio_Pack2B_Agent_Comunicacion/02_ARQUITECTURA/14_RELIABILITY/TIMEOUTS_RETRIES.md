# Timeouts y Reintentos

## Timeouts sugeridos

- Health: 2 s.
- Consulta simple: 10 s.
- Comando local: 30 s.
- Job largo: según tipo.

## Reintentos

Solo para:
- lectura,
- entrega de heartbeat,
- mensajes idempotentes.

No reintentar ciegamente acciones destructivas.

## Backoff

Exponencial con jitter.
