# Permisos NTFS

## Modelo normalizado

- PrincipalSid
- PrincipalName
- PrincipalType
- Rights
- AccessType
- IsInherited
- InheritanceFlags
- PropagationFlags
- AppliesTo
- SourcePath
- OwnerSid
- ObservedAt

## Rights

- FullControl
- Modify
- ReadAndExecute
- ListDirectory
- Read
- Write
- Special

## Reglas

- Deny explícito tiene prioridad.
- Allow explícito se evalúa antes que heredado.
- Los grupos deben expandirse de forma controlada.
- No confundir permisos de share con permisos NTFS.
- Se debe distinguir ACL protegida de ACL heredada.
- No simplificar permisos especiales sin conservar el detalle original.
