# Registro y Enrolamiento

## Flujo de registro

1. Console descubre PC.
2. Agent inicia sin enrolar.
3. Administrador proporciona código/token de enrolamiento.
4. Agent envía identidad y fingerprint.
5. Console aprueba.
6. Se genera relación de confianza.
7. Agent queda `Managed`.

## Estados

- Unregistered
- PendingApproval
- Managed
- Revoked
- Quarantined

## Revocación

La Console puede revocar un Agent.
Un Agent revocado solo debe permitir re-enrolamiento manual.
