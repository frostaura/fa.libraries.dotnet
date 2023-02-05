[<< Back](../README.md)

## Design
The software architecture diagram(s) and design(s).

### Introduction
TODO: Project architecture high-level description goes here.

### Architecture
The architecture of the solution is a layered one. Specifically 3 layers deep. These layers are also only allowed to depend on components adjacent of itself or down (never up - e.g. Engines shouldn't depend on managers). All components should be color coded correctly based on the description below for ease of reading.
| Layer | Description |
| --- | --- |
| Managers (Green) | These components orchestrate other code paths and often fascilitate use cases and as such would be the entry point to the application. |
| Engines (Orange) | These components perform complex operations exclusively. |
| Data Access (Grey) | These components perform IO operations exclusively. |
| Models (Purple) | These are simple data structures (DTOs) / models and occasionally enums. |

### Design Documentation
| Content | Description
| -- | -- |
| [Use Cases](./design.use_cases.md) | Use case diagram(s).
| [High-Level](./design.high_level.md) | High-level system components.
| [Class Diagrams](./design.class.md) | Class diagram(s).
| [Sequence Diagrams](./design.sequence.md) | Sequence diagram(s).

[<< Back](../README.md)