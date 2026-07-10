# API — Impresoras

Base: `/api/v1/printers`

## Consultas

- `GET /`
- `GET /{id}`
- `GET /{id}/endpoints`
- `GET /{id}/queues`
- `GET /{id}/jobs`
- `GET /{id}/consumables`
- `GET /{id}/counters`
- `GET /{id}/maintenance`
- `GET /{id}/alerts`

## Comandos

- `POST /`
- `PATCH /{id}`
- `POST /discover`
- `POST /{id}/maintenance`
- `POST /{id}/refresh`
- `POST /queues/{id}/pause`
- `POST /queues/{id}/resume`
- `POST /queues/{id}/purge`
- `POST /alerts/{id}/acknowledge`

Las acciones sobre colas requieren permisos elevados y confirmación explícita.
