# Form Component Template

## Full Inline Form

```tsx
import { useEffect, useState } from "react";
import { Input } from "@app/components/ui/input";
import { FormActions } from "@shared/components/FormActions";
import { FormError } from "@shared/components/FormError";
import { FormField } from "@shared/components/FormField";
import { FormGrid } from "@shared/components/FormGrid";
import type { {Entity} } from "../types";

interface {Entity}FormProps {
  {entity}?: {Entity};
  onSave: (data: Record<string, unknown>) => Promise<void>;
  onCancel: () => void;
}

function {Entity}Form({ {entity}, onSave, onCancel }: {Entity}FormProps) {
  const isEdit = Boolean({entity});

  // Individual field state
  const [code, setCode] = useState("");
  const [name, setName] = useState("");
  // ... add state for each field

  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  // Populate form for editing
  useEffect(() => {
    if ({entity}) {
      setCode({entity}.code);
      setName({entity}.name);
      // ... set all fields from entity
    } else {
      setCode("");
      setName("");
      // ... reset all fields
    }
  }, [{entity}]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError("");
    try {
      await onSave({
        ...(isEdit ? {} : { code }),
        name,
        // ... include all fields
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : "An error occurred");
    } finally {
      setSaving(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <FormError error={error} />

      {/* Code — disabled when editing */}
      <FormField id="code" label="Code">
        <Input
          id="code"
          value={code}
          onChange={(e) => setCode(e.target.value)}
          disabled={isEdit}
          required
          placeholder="e.g., PROD-001"
        />
      </FormField>

      {/* Name */}
      <FormField id="name" label="Name">
        <Input
          id="name"
          value={name}
          onChange={(e) => setName(e.target.value)}
          required
        />
      </FormField>

      {/* Multi-column layout for related fields */}
      <FormGrid>
        <FormField id="email" label="Email">
          <Input
            id="email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </FormField>
        <FormField id="phone" label="Phone">
          <Input
            id="phone"
            type="tel"
            value={phone}
            onChange={(e) => setPhone(e.target.value)}
          />
        </FormField>
      </FormGrid>

      {/* Numeric fields */}
      <FormField id="creditLimit" label="Credit Limit">
        <Input
          id="creditLimit"
          type="number"
          min="0"
          step="0.01"
          value={creditLimit}
          onChange={(e) => setCreditLimit(e.target.value)}
        />
      </FormField>

      <FormActions onCancel={onCancel} saving={saving} />
    </form>
  );
}
```

## Key Patterns

- **No React Hook Form**: Current codebase uses local `useState` per field (not React Hook Form + Zod yet)
- **`isEdit` flag**: Derived from `Boolean({entity})`, used to disable immutable fields like `code`
- **Error handling**: Catch errors from `onSave`, display via `FormError`
- **Save payload**: Spread code only for create, always include mutable fields
- **Type coercion**: Numeric fields use `type="number"` on `Input`, convert string → number before saving
- **FormGrid**: Groups related fields in 2-column layout (contact info, address fields, etc.)
- **FormActions**: Always last, provides Cancel/Save with loading state

## Numeric Field Conversion

When building the save payload, convert numeric string fields:

```typescript
await onSave({
  ...(isEdit ? {} : { code }),
  name,
  creditLimit: creditLimit ? Number(creditLimit) : undefined,
  rate: rate ? Number(rate) : undefined,
});
```

## Select Dropdowns (for enum/FK fields)

```tsx
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@app/components/ui/select";

<FormField id="currency" label="Currency">
  <Select value={currency} onValueChange={setCurrency}>
    <SelectTrigger>
      <SelectValue placeholder="Select currency" />
    </SelectTrigger>
    <SelectContent>
      <SelectItem value="USD">USD</SelectItem>
      <SelectItem value="EUR">EUR</SelectItem>
    </SelectContent>
  </Select>
</FormField>
```
