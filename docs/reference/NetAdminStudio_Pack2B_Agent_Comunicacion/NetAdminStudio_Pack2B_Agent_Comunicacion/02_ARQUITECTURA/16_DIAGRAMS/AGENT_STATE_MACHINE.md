# Máquina de Estados del Agent

```mermaid
stateDiagram-v2
    [*] --> Starting
    Starting --> Ready
    Starting --> Failed
    Ready --> Degraded
    Degraded --> Ready
    Ready --> Updating
    Updating --> Starting
    Ready --> Stopping
    Degraded --> Stopping
    Stopping --> [*]
```
