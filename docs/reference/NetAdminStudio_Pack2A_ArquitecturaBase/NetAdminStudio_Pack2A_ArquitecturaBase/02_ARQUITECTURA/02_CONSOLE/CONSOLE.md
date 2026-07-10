# Arquitectura de NetAdmin Console

## Objetivo

La Console es el centro de operaciones del administrador.

## Responsabilidades

- Presentar estado.
- Recibir intención del usuario.
- Validar permisos.
- Invocar casos de uso.
- Mostrar progreso y resultados.
- Registrar acciones.

## No debe

- Ejecutar PowerShell directamente.
- Acceder a SQLite directamente.
- Conocer detalles del protocolo Agent.
- Contener reglas de negocio.

## Patrón

MVVM con:
- Views
- ViewModels
- Services
- Navigation
- Dialogs
- State Store

## Servicios de UI

- INavigationService
- IDialogService
- INotificationService
- IClipboardService
- IThemeService
- IUiDispatcher
