import type { ReactNode } from "react";
import { Label } from "@app/components/ui/label";

interface FormFieldProps {
  id?: string;
  label: string;
  labelAction?: ReactNode;
  children: ReactNode;
}

export function FormField({ id, label, labelAction, children }: FormFieldProps) {
  return (
    <div className="space-y-1">
      {labelAction ? (
        <div className="flex items-center justify-between">
          <Label htmlFor={id}>{label}</Label>
          {labelAction}
        </div>
      ) : (
        <Label htmlFor={id}>{label}</Label>
      )}
      {children}
    </div>
  );
}
