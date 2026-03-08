# Frontend Structure (React + shadcn/ui)

## Intent
Frontend is a module-aligned React SPA that consumes versioned backend APIs.

## Folders

- `src/app/`: app bootstrap, router, providers
- `src/modules/`: feature modules
  - `admin/`
  - `master-data/`
  - `finance/`
  - `sales/`
  - `procurement/`
  - `inventory/`
  - `hr/`
  - `reporting/`
- `src/shared/`
  - `api/`: API client and request helpers
  - `components/ui/`: shadcn/ui wrappers and shared UI
  - `hooks/`: reusable hooks
  - `lib/`: utilities
  - `types/`: shared types/contracts
- `src/styles/`: global styles/tokens
- `public/`: static assets

## Next Step
Install dependencies and initialize shadcn/ui after React app bootstrap.
