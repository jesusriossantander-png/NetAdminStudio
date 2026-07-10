# Protocolo de Comunicación

## Transporte MVP

HTTP local sobre Kestrel con JSON.

## Versión

Ruta base:

```text
/api/v1
```

## Endpoints iniciales

- GET `/api/v1/health`
- GET `/api/v1/capabilities`
- GET `/api/v1/inventory`
- POST `/api/v1/commands`
- GET `/api/v1/jobs/{jobId}`
- POST `/api/v1/registration`
- POST `/api/v1/heartbeat`

## Futuro

- HTTPS obligatorio.
- mTLS.
- WebSocket o SignalR para eventos.
- gRPC para alto volumen.
