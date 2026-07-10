# Pack 2A — Arquitectura Base

Este paquete define la arquitectura base de **NetAdmin Studio Enterprise** para que Claude Code pueda crear la solución inicial con una estructura consistente.

## Contenido

- Arquitectura general.
- Estructura de solución.
- Responsabilidades de la Console.
- Responsabilidades del Core.
- Reglas de dependencias.
- Convenciones de proyecto.
- Configuración.
- Logging y observabilidad.
- Diagramas Mermaid.
- ADR iniciales.
- Prompt de implementación.

## Uso recomendado

1. Copiar este paquete dentro del repositorio.
2. Abrir el repositorio en VS Code.
3. Pedir a Claude Code que lea toda la documentación.
4. Ejecutar `08_PROMPTS/020_IMPLEMENTAR_PACK_2A.md`.
5. No avanzar con Agent ni comunicación hasta aprobar esta base.


**Producto:** NetAdmin Studio Enterprise  
**Pack:** 2A — Arquitectura Base  
**Versión:** 1.0  
**Estado:** listo para revisión e implementación inicial

## Reglas globales

- La consola nunca debe ejecutar acciones remotas críticas directamente si existe un Agent disponible.
- La UI no debe contener lógica de negocio.
- Toda acción debe pasar por servicios de aplicación.
- Toda operación modificadora debe registrar auditoría.
- Toda integración debe exponer contratos claros.
- La arquitectura debe permitir pruebas unitarias.
- Las dependencias deben apuntar hacia el núcleo, no al revés.
- El sistema debe funcionar sin Internet dentro de la LAN.
