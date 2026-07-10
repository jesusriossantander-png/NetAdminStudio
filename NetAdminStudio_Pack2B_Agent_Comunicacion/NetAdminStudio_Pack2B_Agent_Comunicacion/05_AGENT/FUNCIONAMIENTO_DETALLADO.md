# Funcionamiento Detallado del Agent

## Inicio

- Verificar servicio.
- Verificar identidad.
- Verificar puerto.
- Inicializar logging.
- Inicializar base local liviana si se usa.
- Publicar estado `Starting`.
- Publicar estado `Ready`.

## Estados

- Starting
- Ready
- Degraded
- Updating
- Stopping
- Failed

## Modo degradado

El Agent puede operar parcialmente si:
- no puede contactar Console,
- falla un collector,
- un módulo de Windows no está disponible.

Debe informar la degradación, no finalizar todo el servicio.
