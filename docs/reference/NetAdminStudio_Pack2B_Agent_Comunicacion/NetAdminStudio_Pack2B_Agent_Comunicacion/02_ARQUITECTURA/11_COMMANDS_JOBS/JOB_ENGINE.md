# Job Engine

## Uso

Operaciones largas o múltiples pasos:

- Instalar impresora.
- Aplicar permisos.
- Generar inventario completo.
- Actualizar Agent.
- Acciones masivas.

## Job

- JobId.
- Type.
- Status.
- Progress.
- StartedAt.
- CompletedAt.
- Steps.
- Result.
- Error.

## Requisitos

- Persistencia.
- Reanudación cuando sea segura.
- Cancelación.
- Progreso.
- Logs por paso.
