import { Button } from "@app/components/ui/button";

interface FormActionsProps {
  onCancel: () => void;
  saving: boolean;
  /** Additional disabled condition for the submit button (beyond `saving`). */
  disabled?: boolean;
  saveLabel?: string;
}

export function FormActions({ onCancel, saving, disabled, saveLabel = "Save" }: FormActionsProps) {
  return (
    <div className="flex justify-end gap-2 pt-2">
      <Button type="button" variant="outline" onClick={onCancel} disabled={saving}>
        Cancel
      </Button>
      <Button type="submit" disabled={saving || disabled}>
        {saving ? "Saving…" : saveLabel}
      </Button>
    </div>
  );
}
