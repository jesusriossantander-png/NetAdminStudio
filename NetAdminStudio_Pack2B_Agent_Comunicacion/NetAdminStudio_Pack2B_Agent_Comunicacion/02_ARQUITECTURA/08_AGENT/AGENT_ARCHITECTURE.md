# Arquitectura del Agent

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


## Responsabilidad

NetAdmin Agent es un Windows Service liviano instalado en cada nodo administrado.

## Subsistemas internos

```text
AgentHost
├── ApiHost
├── Authentication
├── CommandDispatcher
├── JobEngine
├── InventoryCollector
├── HeartbeatService
├── LocalEventStore
├── PlatformAdapters
├── UpdateCoordinator
└── Diagnostics
```

## Reglas

- No incluir UI.
- No mostrar ventanas al usuario.
- Iniciar automáticamente con Windows.
- Usar Generic Host.
- Recuperarse de fallos.
- No bloquear el hilo principal.
- Separar consultas de acciones modificadoras.

## Ciclo de ejecución

1. Cargar configuración.
2. Inicializar identidad.
3. Validar credenciales.
4. Iniciar endpoint local.
5. Registrar heartbeat.
6. Ejecutar inventario inicial.
7. Atender comandos.
8. Procesar Jobs.
9. Mantener logs y eventos.
