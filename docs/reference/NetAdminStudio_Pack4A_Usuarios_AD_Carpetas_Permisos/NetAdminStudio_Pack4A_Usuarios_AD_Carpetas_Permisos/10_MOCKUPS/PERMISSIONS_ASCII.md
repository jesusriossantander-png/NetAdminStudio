# Explorador de permisos

```text
┌──────────────────────────────────────────────────────────────────────────────┐
│ \\SERVIDOR\Ingenieria\Proyectos\2026                         [Analizar acceso]│
├────────────────────────────┬─────────────────────────────────────────────────┤
│ CARPETAS                   │ PERMISOS NTFS                                   │
│ ▾ Ingenieria               │ Principal          Tipo   Derechos   Origen     │
│   ▾ Proyectos              │ Administradores     Allow  Full       Heredado   │
│     ▸ 2025                 │ Ingeniería          Allow  Modify     Explícito  │
│     ▾ 2026                 │ Visitantes          Deny   Write      Explícito  │
│       ▸ Cliente A          │ SYSTEM              Allow  Full       Heredado   │
│       ▸ Cliente B          │                                                 │
├────────────────────────────┴─────────────────────────────────────────────────┤
│ SMB: Ingeniería = Change                                                     │
│ Acceso efectivo para jrios: Modify                                           │
│ Motivo: NTFS Modify + SMB Change                                              │
└──────────────────────────────────────────────────────────────────────────────┘
```
