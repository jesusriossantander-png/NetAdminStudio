# Comunicación Console-Agent

## Modelo

Comunicación local LAN entre Console y Agent.

## Opciones recomendadas para MVP

- HTTP local con Kestrel en el Agent.
- JSON para comandos/respuestas.
- Token compartido inicial.
- TLS en fase posterior.

## Flujo de comando

1. Console genera CommandRequest.
2. Firma o adjunta token.
3. Agent valida.
4. Agent ejecuta.
5. Agent devuelve CommandResult.
6. Console persiste resultado.

## Ejemplo conceptual

```json
{
  "command": "Printer.List",
  "target": "AST-000001",
  "requestId": "GUID"
}
```
