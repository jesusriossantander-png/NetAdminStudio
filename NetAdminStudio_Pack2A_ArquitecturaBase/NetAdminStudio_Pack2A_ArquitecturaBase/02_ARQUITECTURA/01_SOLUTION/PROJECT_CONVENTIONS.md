# Convenciones de Proyecto

## Namespaces

```text
NetAdmin.Core.Assets
NetAdmin.Core.Events
NetAdmin.Application.Assets
NetAdmin.Console.ViewModels
NetAdmin.Data.Repositories
NetAdmin.Platform.Windows.Printers
```

## Convenciones

- Clases: PascalCase.
- Interfaces: prefijo `I`.
- Métodos async: sufijo `Async`.
- DTOs: sufijo `Dto`.
- Requests: sufijo `Request`.
- Responses: sufijo `Response`.
- Commands: sufijo `Command`.
- Queries: sufijo `Query`.

## Resultados

No usar excepciones para flujos normales. Utilizar:

```csharp
Result<T>
OperationError
ValidationError
```

## Nullability

Activar nullable reference types.
