# Contratos de API

## CommandRequest

```json
{
  "commandType": "Printer.Share",
  "parameters": {
    "printerName": "HP LaserJet",
    "shareName": "HP_RECEPCION"
  },
  "requestedBy": "fernando",
  "dryRun": false
}
```

## CommandResponse

```json
{
  "accepted": true,
  "jobId": "guid",
  "status": "Queued"
}
```

## ErrorResponse

```json
{
  "code": "PRINTER_NOT_FOUND",
  "message": "No se encontró la impresora.",
  "technicalDetails": "Get-Printer returned no match.",
  "suggestedAction": "Verifique el nombre instalado.",
  "correlationId": "guid"
}
```
