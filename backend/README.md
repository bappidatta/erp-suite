# Backend Structure (ASP.NET Core Modular Monolith)

## Intent
The backend is a single deployable API host with strict module boundaries.

## Folders

- `src/BuildingBlocks/`
  - `Domain/`
  - `Application/`
  - `Infrastructure/`
  - `Presentation/`
- `src/Modules/`
  - `Admin/`
  - `MasterData/`
  - `Finance/`
  - `Sales/`
  - `Procurement/`
  - `Inventory/`
  - `HR/`
  - `Reporting/`
  Each module contains:
  - `Domain/`
  - `Application/`
  - `Infrastructure/`
  - `Presentation/`
- `src/Host/Api/`: ASP.NET Core entry host
- `tests/Unit/`: unit tests
- `tests/Integration/`: integration tests

## Next Step
Create solution/projects:
- `src/Host/Api` as ASP.NET Core Web API
- module projects per bounded module or feature folder strategy
- test projects in `tests/`
