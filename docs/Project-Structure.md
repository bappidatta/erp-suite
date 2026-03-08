# Project Structure

## Backend

- backend/
  - src/
    - BuildingBlocks/
      - Domain/
      - Application/
      - Infrastructure/
      - Presentation/
    - Modules/
      - Admin/{Domain,Application,Infrastructure,Presentation}
      - MasterData/{Domain,Application,Infrastructure,Presentation}
      - Finance/{Domain,Application,Infrastructure,Presentation}
      - Sales/{Domain,Application,Infrastructure,Presentation}
      - Procurement/{Domain,Application,Infrastructure,Presentation}
      - Inventory/{Domain,Application,Infrastructure,Presentation}
      - HR/{Domain,Application,Infrastructure,Presentation}
      - Reporting/{Domain,Application,Infrastructure,Presentation}
    - Host/Api/
      - Api.csproj
      - Program.cs
      - appsettings.json
      - appsettings.Development.json
  - tests/
    - Unit/
    - Integration/

## Frontend

- frontend/
  - public/
  - src/
    - app/
      - App.tsx
    - modules/
      - admin/
      - master-data/
      - finance/
      - sales/
      - procurement/
      - inventory/
      - hr/
      - reporting/
    - shared/
      - api/
      - components/ui/
      - hooks/
      - lib/
      - types/
    - styles/
      - globals.css
    - main.tsx
  - index.html
  - package.json
  - tsconfig.json
  - vite.config.ts

## Infrastructure

- infra/
  - docker/
  - scripts/

## Notes

- This structure follows modular monolith boundaries for MVP.
- Add concrete module projects and migrations incrementally by feature.
