# API Client Template

## Full Example

```typescript
import { apiFetch } from "@shared/api/client";
import type {
  {Entity},
  Create{Entity}Request,
  Update{Entity}Request,
  PagedResult,
} from "../types";

const {MODULE_UPPER} = "/api/{module}";

// List with server-side filtering, sorting, pagination
export const get{Entities} = (params?: Record<string, string>) => {
  const qs = params ? "?" + new URLSearchParams(params).toString() : "";
  return apiFetch<PagedResult<{Entity}>>(`${{{MODULE_UPPER}}}/{entities}${qs}`);
};

// Get single entity by ID
export const get{Entity}ById = (id: number) =>
  apiFetch<{Entity}>(`${{{MODULE_UPPER}}}/{entities}/${id}`);

// Create new entity
export const create{Entity} = (data: Create{Entity}Request) =>
  apiFetch<{Entity}>(`${{{MODULE_UPPER}}}/{entities}`, {
    method: "POST",
    body: JSON.stringify(data),
  });

// Update existing entity
export const update{Entity} = (id: number, data: Update{Entity}Request) =>
  apiFetch<{Entity}>(`${{{MODULE_UPPER}}}/{entities}/${id}`, {
    method: "PUT",
    body: JSON.stringify(data),
  });

// Soft delete entity
export const delete{Entity} = (id: number) =>
  apiFetch<void>(`${{{MODULE_UPPER}}}/{entities}/${id}`, { method: "DELETE" });

// Activate entity
export const activate{Entity} = (id: number) =>
  apiFetch<{ message: string }>(`${{{MODULE_UPPER}}}/{entities}/${id}/activate`, {
    method: "POST",
  });

// Deactivate entity
export const deactivate{Entity} = (id: number) =>
  apiFetch<{ message: string }>(`${{{MODULE_UPPER}}}/{entities}/${id}/deactivate`, {
    method: "POST",
  });
```

## Query Parameter Conventions

The backend accepts these standard query params (matching `Get{Entity}Query`):

| Param | Type | Purpose |
|-------|------|---------|
| `searchTerm` | `string` | Full-text search across searchable fields |
| `isActive` | `"true"` \| `"false"` | Filter by active status |
| `sortBy` | `string` | Column name to sort by |
| `sortDescending` | `"true"` \| `"false"` | Sort direction |
| `page` | `string` | Page number (1-based) |
| `pageSize` | `string` | Items per page (default 20) |

Entity-specific filters (e.g., `currency`, `type`) are added as additional params.
