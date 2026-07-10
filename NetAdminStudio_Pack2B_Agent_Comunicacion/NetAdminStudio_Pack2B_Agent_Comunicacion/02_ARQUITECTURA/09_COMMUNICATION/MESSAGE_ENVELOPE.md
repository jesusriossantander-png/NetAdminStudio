# Sobre de Mensajes

## Envelope común

```json
{
  "messageId": "guid",
  "correlationId": "guid",
  "causationId": "guid",
  "messageType": "Printer.Share.Command",
  "schemaVersion": "1.0",
  "sentAtUtc": "2026-07-10T00:00:00Z",
  "source": "console-id",
  "target": "agent-id",
  "payload": {}
}
```

## Reglas

- `messageId` único.
- `correlationId` une toda la operación.
- `causationId` identifica mensaje causante.
- Hora siempre UTC.
- `schemaVersion` obligatorio.
- Payload validado antes de ejecutar.
