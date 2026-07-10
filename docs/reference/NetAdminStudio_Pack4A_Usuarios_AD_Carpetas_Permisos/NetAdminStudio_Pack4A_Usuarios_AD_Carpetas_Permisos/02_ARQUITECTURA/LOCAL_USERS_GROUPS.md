# Usuarios y grupos locales

## Fuentes

- Win32_UserAccount
- Win32_Group
- Win32_GroupUser
- LocalAccounts API
- SAM/NetAPI según adaptador

## Datos

- SID;
- nombre;
- nombre completo;
- descripción;
- habilitada;
- bloqueada;
- contraseña requerida;
- contraseña expira;
- última autenticación disponible;
- grupos;
- perfil;
- tipo de cuenta.

## Reglas

- SID es la identidad primaria.
- El nombre puede cambiar.
- Una cuenta eliminada no se borra del historial.
- Las cuentas integradas se etiquetan.
- La cuenta Administrador integrada se detecta por SID, no por nombre.
