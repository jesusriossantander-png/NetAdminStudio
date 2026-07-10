# API — Red

Base: `/api/v1/network`

## Consultas

- `GET /assets`
- `GET /assets/{id}`
- `GET /assets/{id}/interfaces`
- `GET /assets/{id}/ports`
- `GET /assets/{id}/metrics`
- `GET /assets/{id}/alerts`
- `GET /topology`
- `GET /scans`
- `GET /scans/{id}`

## Comandos

- `POST /assets`
- `PATCH /assets/{id}`
- `POST /scans`
- `POST /diagnostics/ping`
- `POST /diagnostics/traceroute`
- `POST /diagnostics/dns`
- `POST /diagnostics/tcp-connect`
- `POST /wake-on-lan`
- `POST /alerts/{id}/acknowledge`

## Respuesta de operación larga

```json
{
  "jobId": "guid",
  "state": "Queued",
  "correlationId": "guid",
  "statusUrl": "/api/v1/jobs/guid"
}
```

## Filtros

- query
- type
- state
- locationId
- sectorId
- criticality
- source
- lastSeenFrom
- page
- pageSize
- sort
