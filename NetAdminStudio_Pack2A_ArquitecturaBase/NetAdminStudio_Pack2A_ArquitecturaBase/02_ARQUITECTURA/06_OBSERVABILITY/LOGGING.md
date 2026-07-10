# Logging

## Tecnología sugerida

Serilog.

## Niveles

- Verbose
- Debug
- Information
- Warning
- Error
- Fatal

## Campos estructurados

- Timestamp
- CorrelationId
- User
- SourcePc
- TargetAsset
- Module
- Action
- Result
- DurationMs

## Ubicación

```text
C:\ProgramData\NetAdminStudio\Logs
```

## Regla

Nunca registrar contraseñas, tokens o secretos.
