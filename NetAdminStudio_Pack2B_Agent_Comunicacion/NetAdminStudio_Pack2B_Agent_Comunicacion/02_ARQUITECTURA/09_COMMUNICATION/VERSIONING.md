# Versionado del Protocolo

## Estrategia

- Versionar rutas mayores: `/api/v1`.
- Versionar esquemas en mensajes.
- Mantener compatibilidad hacia atrás durante al menos una versión mayor.
- La Console consulta capacidades y versión antes de enviar comandos.

## Incompatibilidad

Si una acción no es compatible:
- no ejecutarla,
- mostrar razón,
- sugerir actualización del Agent.
