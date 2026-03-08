# ERP Suite

ERP Suite is an MVP ERP built as a web application with a modular monolith architecture.

- Backend: ASP.NET Core + EF Core + PostgreSQL
- Frontend: React + Vite + shadcn/ui foundation
- Architecture: Clean Architecture with module boundaries

## Prerequisites

- .NET SDK 10+
- Node.js 20+
- Docker Desktop (running)

## Quick Start

1. Start PostgreSQL and pgAdmin

```powershell
cd infra/docker
docker compose up -d
```

2. Apply database migration

```powershell
cd ../../backend/src/Host/Api
dotnet ef database update --project ../../Modules/Admin/Infrastructure/ErpSuite.Modules.Admin.Infrastructure.csproj --context ErpDbContext
```

3. Run backend API

```powershell
cd ../../Host/Api
dotnet run
```

4. Run frontend app in a separate terminal

```powershell
cd frontend
npm install
npm run dev
```

5. Open app

- Frontend: http://localhost:5173
- Backend API health: http://localhost:5000
- OpenAPI (development): http://localhost:5000/openapi/v1.json
- pgAdmin: http://localhost:5050
- pgAdmin login: admin@erpsuite.dev / admin

## Default Login (Seeded)

- Email: admin@erpsuite.local
- Password: Admin@123

This user is seeded automatically at API startup if no users exist.

## Useful Commands

Build backend:

```powershell
cd backend/src/Host/Api
dotnet build
```

Build frontend:

```powershell
cd frontend
npm run build
```

Stop docker services:

```powershell
cd infra/docker
docker compose down
```

## Top-level Structure

- backend/: API host, modules, and tests
- frontend/: React SPA and shared UI foundations
- docs/: PRD, architecture guardrails, implementation plan
- infra/: Docker and infra scripts

## Troubleshooting

Docker command not recognized in PowerShell:

- Restart terminal after Docker installation, or
- Add Docker binary path to the current shell:

```powershell
$env:Path += ";C:\Program Files\Docker\Docker\resources\bin"
```

API build fails because dll file is locked:

- Stop previously running dotnet run process, then build again.

Frontend login fails:

- Confirm backend is running on port 5000.
- Confirm database migration is applied.
- Confirm default admin user exists (seed runs on startup).

pgAdmin page does not open:

- Run `docker compose down` then `docker compose up -d` from `infra/docker`.
- Verify container status with `docker ps` and ensure `erp-suite-pgadmin` is `Up`.
- Use valid pgAdmin credentials: `admin@erpsuite.dev` / `admin`.

## Reference Docs

- docs/PRD-ERP-MVP.md
- docs/MVP-Architecture-Guardrails.md
- docs/Implementation-Plan.md
- docs/Project-Structure.md
