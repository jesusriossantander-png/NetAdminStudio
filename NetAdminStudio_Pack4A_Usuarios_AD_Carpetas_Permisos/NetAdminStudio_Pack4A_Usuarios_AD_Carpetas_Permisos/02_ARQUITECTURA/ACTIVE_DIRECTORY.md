# Active Directory

## Objetivos

- leer usuarios y grupos;
- consultar membresías;
- detectar cuentas deshabilitadas, bloqueadas o inactivas;
- identificar privilegios;
- relacionar usuarios con recursos;
- no depender de AD en el dominio central.

## Adaptadores posibles

- System.DirectoryServices.Protocols
- LDAP seguro
- PowerShell tipado y restringido
- WMI/CIM para datos locales
- Win32 API

## Entidades

- DirectoryDomain
- DirectoryUser
- DirectoryGroup
- DirectoryComputer
- DirectoryMembership
- DirectoryOrganizationalUnit

## Atributos mínimos de usuario

- objectGuid
- sid
- samAccountName
- userPrincipalName
- displayName
- enabled
- locked
- passwordExpired
- passwordNeverExpires
- lastLogonTimestamp
- accountExpires
- distinguishedName
- department
- title
- mail
- manager
- sourceDomain

## Seguridad

- usar LDAP Signing/Channel Binding cuando aplique;
- preferir LDAPS;
- no almacenar credenciales;
- guardar referencia a secreto;
- aplicar mínimo privilegio;
- no habilitar cambios AD en el MVP por defecto.
