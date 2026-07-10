# Estructura de la Solución

## Estructura propuesta

```text
NetAdminStudio.sln
/src
  /NetAdmin.Console
  /NetAdmin.Application
  /NetAdmin.Core
  /NetAdmin.Data
  /NetAdmin.Communication
  /NetAdmin.Platform.Windows
  /NetAdmin.Reporting
  /NetAdmin.Agent
/tests
  /NetAdmin.Core.Tests
  /NetAdmin.Application.Tests
  /NetAdmin.Data.Tests
  /NetAdmin.Console.Tests
/docs
/build
/scripts
```

## Responsabilidad por proyecto

### NetAdmin.Console
WPF, ViewModels, navegación, estilos.

### NetAdmin.Application
Casos de uso y servicios de aplicación.

### NetAdmin.Core
Dominio puro.

### NetAdmin.Data
SQLite, repositorios, migraciones.

### NetAdmin.Communication
DTOs y clientes de comunicación.

### NetAdmin.Platform.Windows
Integraciones específicas de Windows.

### NetAdmin.Reporting
Generación de reportes.

### NetAdmin.Agent
Servicio Windows.
