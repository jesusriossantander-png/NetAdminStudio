# Pack 2B — Agent y Comunicación

Este paquete especifica el corazón operativo de NetAdmin Studio: el Agent, el protocolo Console-Agent, descubrimiento, inventario, heartbeat, comandos, jobs, confiabilidad y seguridad del canal.

## Contenido

- Arquitectura detallada del Agent.
- Registro y enrolamiento.
- Protocolo HTTP/JSON versionado.
- Contratos de mensajes.
- Descubrimiento de red.
- Heartbeat y presencia.
- Inventario completo e incremental.
- Command Dispatcher.
- Job Engine.
- Timeouts, reintentos e idempotencia.
- Seguridad del canal.
- Diagramas Mermaid.
- Pruebas.
- ADR.
- Prompt de implementación para Claude Code.

## Orden recomendado

1. Leer todo el Pack 2A.
2. Leer este Pack 2B.
3. Ejecutar `08_PROMPTS/030_IMPLEMENTAR_PACK_2B.md`.
4. Implementar primero comunicación mínima y heartbeat.
5. Agregar inventario.
6. Agregar Commands y Jobs.
7. Recién después sumar módulos funcionales.


**Producto:** NetAdmin Studio Enterprise  
**Pack:** 2B — Agent, Comunicación, Descubrimiento e Inventario  
**Versión:** 1.0  
**Estado:** especificación técnica lista para revisión e implementación

## Reglas globales

- El Agent ejecuta acciones locales; la Console orquesta.
- Todo comando remoto debe estar autenticado, autorizado, correlacionado y auditado.
- Las operaciones largas se modelan como Jobs.
- Los mensajes deben ser versionables.
- Los reintentos nunca deben duplicar efectos.
- Los inventarios deben diferenciar snapshot completo de cambios incrementales.
- El Agent debe continuar funcionando aunque la Console esté desconectada.
- El sistema debe operar en LAN sin depender de Internet.
