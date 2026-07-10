# Command Dispatcher

## Responsabilidad

- Validar comando.
- Validar autorización.
- Resolver handler.
- Determinar ejecución inmediata o Job.
- Registrar auditoría.
- Devolver resultado.

## Contrato

```csharp
public interface ICommandHandler<TCommand, TResult>
{
    Task<Result<TResult>> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken);
}
```

## Regla

Cada comando tiene un único handler.
