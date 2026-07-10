# Almacenamiento Local del Agent

## Objetivo

Persistir datos mínimos para resiliencia.

## Contenido

- Identidad.
- Configuración.
- Jobs pendientes.
- Resultados no entregados.
- Último inventario.
- Logs locales.
- Paquetes de actualización temporales.

## Motor

Para MVP puede usarse JSON + archivos.
Si crece, usar SQLite local separada de la Console.

## Límites

- Purga automática.
- Cifrado/protección de secretos.
- No almacenar contraseñas de usuarios.
