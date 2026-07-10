# NetAdmin Studio Documentation Pack v1.0

Este paquete contiene la documentación base completa para iniciar el desarrollo de **NetAdmin Studio Enterprise** con Claude Code en VS Code.

## Cómo usar

1. Descomprimir este ZIP.
2. Abrir la carpeta raíz en VS Code.
3. Abrir Claude Code.
4. Usar el archivo `08_PROMPTS/000_PROMPT_INICIAL_CLAUDE.md`.
5. Pedir primero análisis y estructura, no código directo.

## Estructura

- `00_MASTER`: visión, filosofía, objetivos, principios y roadmap.
- `01_PRODUCTO`: requisitos de producto, casos de uso, personas y funcionalidades.
- `02_ARQUITECTURA`: arquitectura técnica completa.
- `03_DATABASE`: diseño de base de datos, historial y auditoría.
- `04_UI`: diseño funcional de pantallas y módulos.
- `05_AGENT`: especificación del agente Windows.
- `06_SECURITY`: seguridad, permisos, NTFS/SMB, tokens y certificados.
- `07_TESTING`: plan de pruebas.
- `08_PROMPTS`: prompts para Claude Code por fase.
- `09_DECISIONES`: decisiones arquitectónicas ADR.
- `10_MOCKUPS`: mockups textuales y guías visuales.


**Producto:** NetAdmin Studio Enterprise  
**Versión documentación:** 1.0  
**Objetivo:** servir como documentación completa inicial para que Claude Code trabaje dentro de VS Code con contexto claro, arquitectura modular y reglas de calidad.

## Reglas globales del proyecto

- Diagnosticar antes de modificar.
- Confirmar cambios críticos.
- Crear backup antes de modificar permisos, recursos compartidos o configuración de red.
- Registrar toda acción en logs y auditoría.
- Separar UI, lógica de negocio, acceso a datos y comunicación.
- No depender de Internet para funciones críticas.
- Priorizar seguridad sobre comodidad.
- Mantener compatibilidad con Windows 10 y Windows 11.
- Diseñar pensando en crecimiento futuro: Linux, Raspberry, NAS, Docker, Hyper-V, Active Directory y plugins.
