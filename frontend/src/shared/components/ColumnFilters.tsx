import { Input } from "@app/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@app/components/ui/select";

interface FilterOption {
  value: string;
  label: string;
}

interface ColumnFilterInputProps {
  value: string;
  onChange: (value: string) => void;
  placeholder?: string;
}

export function ColumnFilterInput({ value, onChange, placeholder }: ColumnFilterInputProps) {
  return (
    <Input
      value={value}
      onChange={(e) => onChange(e.target.value)}
      placeholder={placeholder}
      className="h-8 text-xs"
    />
  );
}

interface ColumnFilterSelectProps {
  value: string;
  onChange: (value: string) => void;
  options: FilterOption[];
  placeholder?: string;
}

export function ColumnFilterSelect({ value, onChange, options, placeholder }: ColumnFilterSelectProps) {
  // Find the label for the current value
  const selectedLabel = options.find((opt) => opt.value === value)?.label || placeholder;

  return (
    <Select value={value} onValueChange={(v) => onChange(v ?? "")}>
      <SelectTrigger className="h-8 text-xs w-full">
        <SelectValue placeholder={placeholder}>
          <span className="text-xs">{value ? selectedLabel : placeholder}</span>
        </SelectValue>
      </SelectTrigger>
      <SelectContent>
        {options.map((option) => (
          <SelectItem key={option.value} value={option.value}>
            {option.label}
          </SelectItem>
        ))}
      </SelectContent>
    </Select>
  );
}
