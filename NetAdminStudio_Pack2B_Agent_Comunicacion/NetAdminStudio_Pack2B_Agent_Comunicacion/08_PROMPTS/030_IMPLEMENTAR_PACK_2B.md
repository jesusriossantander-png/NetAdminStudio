# Prompt para Implementar Pack 2B

Leé Pack 2A y Pack 2B completos antes de modificar código.

## Objetivo

Implementar la primera versión funcional de Console-Agent.

## Entrega

1. Agent como Windows Service usando Generic Host.
2. Kestrel local.
3. Endpoints:
   - health,
   - capabilities,
   - registration,
   - heartbeat,
   - inventory,
   - commands,
   - jobs.
4. Envelope versionado.
5. Token protegido con DPAPI.
6. Command Dispatcher.
7. Job Engine persistente básico.
8. Idempotency store.
9. Collectors básicos:
   - sistema,
   - SO,
   - red,
   - discos,
   - impresoras.
10. Cliente Agent en Console.
11. Registro de Agent.
12. Heartbeat y estado Online/Offline.
13. Persistencia de inventario en Console.
14. Logs estructurados y CorrelationId.
15. Pruebas unitarias e integración.

## Primer comando de prueba

Crear un comando seguro de lectura:

`System.GetSummary`

Luego un comando modificador controlado:

`Service.Restart` limitado inicialmente a `Spooler`.

## Restricciones

- No implementar todavía usuarios, NTFS o SMB.
- No exponer endpoint sin autenticación, excepto health mínimo.
- No usar secretos hardcodeados.
- No bloquear la UI.
- No asumir que la Console siempre está online.
- La solución debe compilar y las pruebas deben pasar.

Antes de programar, entregá:
- plan,
- archivos a crear/modificar,
- contratos propuestos,
- riesgos,
- criterios de aceptación.
