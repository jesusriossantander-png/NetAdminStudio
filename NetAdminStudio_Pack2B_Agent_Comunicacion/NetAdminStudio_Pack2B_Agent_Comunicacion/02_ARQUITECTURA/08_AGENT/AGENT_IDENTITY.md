# Identidad del Agent

## Identidad

Cada Agent tiene identidad estable, independiente de IP y nombre de PC.

## Campos

- AgentId: GUID.
- AssetId: identificador lógico.
- InstallationId: GUID de instalación.
- MachineFingerprint: hash de señales no sensibles.
- AgentVersion.
- FirstRegisteredAt.
- LastRegisteredAt.

## Persistencia

Guardar en:

```text
C:\ProgramData\NetAdminStudio\Agent\identity.json
```

El archivo debe protegerse con ACL restrictiva.

## Regla

Reinstalar no debe crear un activo duplicado si la Console puede correlacionarlo de forma segura.
