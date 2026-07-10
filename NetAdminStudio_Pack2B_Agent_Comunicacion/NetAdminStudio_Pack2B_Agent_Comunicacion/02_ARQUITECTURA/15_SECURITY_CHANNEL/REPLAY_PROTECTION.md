# Protección contra Repetición

## Riesgo

Un atacante podría repetir un comando capturado.

## Mitigaciones

- MessageId único.
- Timestamp.
- Ventana de validez.
- Nonce.
- Firma/HMAC.
- IdempotencyKey.

## MVP

Implementar al menos:
- MessageId,
- Timestamp,
- expiración,
- token.
