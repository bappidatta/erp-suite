---
name: erp-frontend-feature
description: "Scaffold and implement a complete frontend CRUD feature in the ERP Suite React app. Use when adding a new page, API client, types, or form to any module (Sales, Finance, Procurement, HR, Inventory, Admin, Reporting). Covers TypeScript interfaces, API client functions, CRUD page with DataTable, inline form dialog, route registration, and sidebar navigation."
argument-hint: "Describe the feature, entity name, and target module (e.g., 'Add Products page to Inventory module')"
---

# ERP Frontend Feature Implementation

## When to Use

- Adding a new CRUD page with table + form to any frontend module
- Implementing a new entity list/detail UI that mirrors a backend feature
- Scaffolding the frontend boilerplate for a new entity following established patterns

## Pre-Flight

1. Confirm with the user: **entity name**, **target module**, and **key fields**
2. If the user hasn't specified fields, ask what columns the table needs
3. Verify the module exists under `frontend/src/modules/{module-name}/`
4. Check that the backend API endpoints exist (the frontend types must match backend DTOs)

## Procedure

Follow these steps in order. Each step produces one or more files.

### Step 1 — TypeScript Interfaces

Create or update `frontend/src/modules/{module-name}/types.ts`

Define these interfaces to match the backend DTOs exactly:

| Interface | Purpose |
|-----------|---------|
| `{Entity}` | Response type — all readable fields including `id`, audit fields |
| `Create{Entity}Request` | Required fields as required props, optional fields with `?` |
| `Update{Entity}Request` | Mutable fields only (typically no `code`), optional with `?` |
| `PagedResult<T>` | Re-export if not already in the module's types |

Rules:
- Interface property names must be **camelCase** matching the backend JSON serialization
- Use `number` for C# `long`, `decimal`, `int`
- Use `string` for C# `DateTime` (ISO strings from API)
- Use `string` for C# enums (or a union type if known values are fixed)
- Mark nullable backend fields as optional with `?`
- If `PagedResult<T>` already exists in the types file, don't duplicate it

Reference: [Types template](./references/types.md)

### Step 2 — API Client Functions

Create or update `frontend/src/modules/{module-name}/api/{module}Api.ts`

Standard CRUD functions:

```typescript
get{Entities}(params?)       → PagedResult<{Entity}>
get{Entity}ById(id)          → {Entity}
create{Entity}(data)         → {Entity}
update{Entity}(id, data)     → {Entity}
delete{Entity}(id)           → void
activate{Entity}(id)         → { message: string }
deactivate{Entity}(id)       → { message: string }
```

Rules:
- Import `apiFetch` from `@shared/api/client`
- Import types from `../types`
- Base path constant: `const {MODULE} = "/api/{module}"` (lowercase)
- Query params via `new URLSearchParams(params).toString()`
- POST/PUT: `method`, `body: JSON.stringify(data)`
- DELETE: `method: "DELETE"`
- Activate/Deactivate: `method: "POST"` to `/{id}/activate` or `/{id}/deactivate`

Reference: [API client template](./references/api-client.md)

### Step 3 — CRUD Page Component

Create `frontend/src/modules/{module-name}/pages/{Entities}Page.tsx`

This is the main page component containing the table, form dialog, and delete dialog.

Structure:
1. **State**: `result`, `loading`, `columnFilters`, `sorting`, `page`, `formOpen`, `editing{Entity}`, `deleteTarget`
2. **Fetch function**: `useCallback` that builds query params from filters/sorting/page, calls API, sets result
3. **useEffect**: triggers fetch when dependencies change
4. **Column definitions**: `ColumnDef<{Entity}>[]` with filter components and sort
5. **CRUD handlers**: `handleSave`, `handleDelete`, `handleActivate`, `handleDeactivate`
6. **Render**: `PageLayout` > `PageHeader` + `DataTable` + `Dialog` (form) + `DeleteDialog`

Rules:
- Import shared components from `@shared/components`
- Import shadcn/ui components from `@app/components/ui/*`
- Import icons from `lucide-react`
- Use `Dialog` / `DialogContent` / `DialogHeader` / `DialogTitle` for the form modal
- Include a `StatusBadge` column for `isActive` entities
- Include an actions column with Edit, Delete, Activate/Deactivate buttons
- Use `ColumnFilterInput` for text search filters
- Use `ColumnFilterSelect` for enum/status filters
- Export the page as a named export: `export function {Entities}Page()`

Reference: [Page component template](./references/page-component.md)

### Step 4 — Inline Form Component

Define the form as a component inside the page file (or a separate file if complex).

Structure:
1. **Props**: `{ {entity}?: {Entity}; onSave: (data) => Promise<void>; onCancel: () => void }`
2. **State**: individual `useState` for each field, `saving`, `error`
3. **Init effect**: populate form fields from `{entity}` prop when editing
4. **`handleSubmit`**: prevents default, sets saving, calls `onSave`, catches errors
5. **Render**: `<form>` with `FormField`, `FormGrid`, `FormActions`, `FormError`

Rules:
- Use `FormField` wrapper with `Label` for each input
- Use `FormGrid` for multi-column layouts (2 cols default)
- Use `FormActions` for Cancel/Save buttons
- Use `FormError` to display API errors
- `Input` from `@app/components/ui/input` for text fields
- `Select` from `@app/components/ui/select` for dropdowns
- Disable `code` field when editing (codes are immutable)
- Set appropriate `type` on inputs: `email`, `number`, `tel`, `url`

Reference: [Form component template](./references/form-component.md)

### Step 5 — Route Registration

Update `frontend/src/app/App.tsx`:

1. Add import: `import { {Entities}Page } from "@modules/{module-name}/pages/{Entities}Page"`
2. Add route inside the `<ProtectedRoute>` block: `<Route path="/{module}/{entities}" element={<{Entities}Page />} />`

Route path convention: `/{module-name}/{entities-kebab-case}` (e.g., `/inventory/products`, `/finance/tax-codes`)

### Step 6 — Sidebar Navigation

Update `frontend/src/app/components/layout/Sidebar.tsx`:

1. Find the `navItems` array
2. Locate the module's navigation group (or create one if first feature in module)
3. Add submenu entry: `{ label: "{Entities}", href: "/{module}/{entities}" }`
4. If creating a new module group, add appropriate icon from `lucide-react`

Existing module icons:
- Admin: `Users`
- Sales: `DollarSign`
- Procurement: `Truck`
- Finance: `FileText`
- Inventory: `Package`

### Step 7 — Build & Verify

Run from `frontend/`:
```bash
npm run build
```

Fix any TypeScript or build errors before completing.

## Available Shared Components

Use these from `@shared/components`:

| Component | Import | Purpose |
|-----------|--------|---------|
| `PageLayout` | `@shared/components` | Page container with vertical spacing |
| `PageHeader` | `@shared/components` | Title + description + action button |
| `DataTable` | `@shared/components` | TanStack table with filters, sort, pagination |
| `DeleteDialog` | `@shared/components` | Confirmation dialog for deletions |
| `FormField` | `@shared/components` | Label + input wrapper |
| `FormGrid` | `@shared/components` | 2/3 column form layout |
| `FormActions` | `@shared/components` | Cancel/Save button pair |
| `FormError` | `@shared/components` | Error message display |
| `StatusBadge` | `@shared/components` | Active/Inactive badge |
| `ColumnFilterInput` | `@shared/components/ColumnFilters` | Text filter for table columns |
| `ColumnFilterSelect` | `@shared/components/ColumnFilters` | Select filter for table columns |

## Available shadcn/ui Components

Import from `@app/components/ui/{name}`:
`alert`, `badge`, `button`, `card`, `collapsible`, `dialog`, `dropdown-menu`, `input`, `label`, `scroll-area`, `select`, `separator`, `sheet`, `table`, `tabs`
