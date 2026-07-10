# NetAdminStudio — Pack 3A: Red e Impresoras

## Propósito

Este pack define la arquitectura funcional, técnica y visual de los módulos **Red** e **Impresoras** de NetAdminStudio.

Mantiene las decisiones establecidas por los packs anteriores:

- arquitectura limpia pragmática;
- WPF + MVVM para la consola;
- SQLite para el MVP;
- agente autónomo;
- comunicación HTTP/JSON versionada;
- operaciones largas representadas como jobs;
- auditoría permanente;
- seguridad progresiva;
- funcionamiento degradado y recuperación;
- diseño consistente con NetAdminStudio Foundation.

## Alcance

### Red

- inventario de dispositivos;
- descubrimiento controlado;
- presencia y disponibilidad;
- topología;
- interfaces y puertos;
- VLAN;
- DHCP y DNS como información observada;
- herramientas de diagnóstico;
- historial y alertas;
- integración SNMP progresiva.

### Impresoras

- inventario;
- estado operativo;
- colas;
- trabajos;
- consumibles;
- contadores;
- mantenimiento;
- alertas;
- impresoras USB compartidas mediante host Windows o Raspberry Pi;
- integración futura con CUPS, WMI, IPP y SNMP.

## Principios

1. Observar antes de administrar.
2. No realizar cambios de red por defecto.
3. Separar datos observados de datos configurados manualmente.
4. Conservar historial.
5. Explicar el origen y nivel de confianza de cada dato.
6. Evitar escaneos agresivos.
7. Representar operaciones largas mediante jobs.
8. Proteger credenciales y secretos.
9. Mantener trazabilidad completa.
10. Permitir operación parcial cuando un origen no esté disponible.

## Estructura

- `03_DATABASE`: modelo y persistencia.
- `04_UI`: experiencia de usuario.
- `07_TESTING`: pruebas y aceptación.
- `08_PROMPTS`: implementación y revisión.
- `09_DECISIONES`: ADR.
- `10_MOCKUPS`: vistas ASCII.
- `11_MODULES`: especificación funcional.
- `12_CONTRACTS`: contratos API y eventos.
- `13_SECURITY`: seguridad.
- `14_OPERATIONS`: operación y soporte.

## Fuera de alcance del Pack 3A

- edición automática de configuración de switches, routers o firewalls;
- instalación remota de drivers;
- ejecución de cambios masivos;
- correlación avanzada mediante IA;
- monitoreo NetFlow/sFlow;
- controlador SDN;
- alta disponibilidad del servidor central.

Estos puntos quedan preparados para packs posteriores.
