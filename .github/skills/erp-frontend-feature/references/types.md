# Types Template

## Entity Response Interface

```typescript
export interface {Entity} {
  id: number;
  code: string;
  name: string;
  // ... entity-specific fields
  isActive: boolean;
  notes?: string;
  createdAt: string;
  createdBy: string;
  updatedAt?: string;
}
```

## Create Request Interface

```typescript
export interface Create{Entity}Request {
  code: string;          // Required — immutable after creation
  name: string;          // Required
  // ... required fields without ?
  // ... optional fields with ?
  isActive?: boolean;    // Defaults to true on backend
  notes?: string;
}
```

## Update Request Interface

```typescript
export interface Update{Entity}Request {
  name: string;          // Required — no code (immutable)
  // ... mutable fields
  isActive?: boolean;
  notes?: string;
}
```

## PagedResult Generic

Only add this if the module's `types.ts` doesn't already have it:

```typescript
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
```

## Type Mapping Rules

| C# Type | TypeScript Type |
|---------|-----------------|
| `long`, `int` | `number` |
| `decimal` | `number` |
| `string` | `string` |
| `bool` | `boolean` |
| `DateTime`, `DateTime?` | `string` (ISO format) |
| `enum` | `string` or union type |
| Nullable (`?` in C#) | Optional (`?` in TS) |
