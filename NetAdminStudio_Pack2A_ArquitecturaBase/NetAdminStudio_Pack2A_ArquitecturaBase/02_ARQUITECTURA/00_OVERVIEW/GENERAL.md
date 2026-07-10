# Arquitectura General

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


## Objetivo

Definir una base técnica modular para construir una plataforma de administración de infraestructura.

## Componentes principales

```text
NetAdmin.Console
NetAdmin.Agent
NetAdmin.Core
NetAdmin.Application
NetAdmin.Data
NetAdmin.Communication
NetAdmin.Platform.Windows
NetAdmin.Reporting
NetAdmin.Installer
NetAdmin.Updater
```

## Capas

### Presentación
- WPF.
- MVVM.
- Navegación.
- Componentes visuales.

### Aplicación
- Casos de uso.
- Orquestación.
- Validaciones.
- Resultados.

### Dominio/Core
- Entidades.
- Value Objects.
- Contratos.
- Eventos de dominio.
- Enumeraciones.

### Infraestructura
- SQLite.
- Windows APIs.
- PowerShell.
- WMI/CIM.
- File system.
- Logging.

## Principio rector

Las capas externas dependen de las internas. El núcleo no conoce WPF, SQLite, PowerShell ni APIs de Windows.
