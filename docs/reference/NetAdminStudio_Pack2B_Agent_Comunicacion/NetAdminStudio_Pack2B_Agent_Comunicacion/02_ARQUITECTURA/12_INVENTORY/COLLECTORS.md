# Collectors de Inventario

## Diseño

Cada categoría usa un collector independiente.

```csharp
public interface IInventoryCollector
{
    string Name { get; }
    Task<InventorySectionResult> CollectAsync(
        CancellationToken cancellationToken);
}
```

## Ventajas

- Fallos aislados.
- Ejecución paralela controlada.
- Plugins futuros.
- Versionado por collector.

## Regla

Un collector fallido no invalida todo el inventario.
