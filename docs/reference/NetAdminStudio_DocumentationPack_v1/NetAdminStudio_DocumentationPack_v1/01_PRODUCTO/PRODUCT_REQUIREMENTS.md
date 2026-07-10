# Product Requirements Document

## Resumen

NetAdmin Studio Enterprise permite administrar infraestructura Windows desde una consola central.

## Alcance inicial

- Windows 10/11 en grupo de trabajo.
- Administración mediante Agent local.
- Red LAN.
- Base de datos SQLite local.
- Consola WPF.

## Fuera de alcance inicial

- Active Directory completo.
- Linux.
- Panel web.
- App móvil.
- IA avanzada.
- Cloud multiempresa.

## Requisitos clave

### RQ-001 Descubrimiento
La consola debe descubrir equipos en el rango local usando ping, ARP, NetBIOS y puertos.

### RQ-002 Agent
El agente debe instalarse como servicio Windows e iniciar con el sistema.

### RQ-003 Inventario
El agente debe reportar nombre, IP, MAC, usuario, Windows, CPU, RAM, discos, impresoras, carpetas compartidas, usuarios y grupos.

### RQ-004 Impresoras
Debe permitir compartir, dejar de compartir, limpiar cola y reiniciar Spooler.

### RQ-005 Carpetas
Debe listar recursos SMB y permisos NTFS/SMB.

### RQ-006 Usuarios
Debe crear, deshabilitar, activar usuarios y administrar grupos.

### RQ-007 Seguridad
Toda acción remota debe autenticarse y auditarse.

### RQ-008 Auditoría
Cada cambio debe registrar actor, destino, acción, estado anterior y nuevo.

### RQ-009 Backup
Antes de modificar ACL o SMB, debe respaldar permisos.

### RQ-010 Modo limitado
Si no hay agente, mostrar información básica y marcar funciones limitadas.
