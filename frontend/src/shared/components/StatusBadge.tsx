import { Badge } from "@app/components/ui/badge";

interface StatusBadgeProps {
  isActive: boolean;
  activeLabel?: string;
  inactiveLabel?: string;
}

export function StatusBadge({
  isActive,
  activeLabel = "Active",
  inactiveLabel = "Inactive",
}: StatusBadgeProps) {
  return (
    <Badge variant={isActive ? "default" : "secondary"}>
      {isActive ? activeLabel : inactiveLabel}
    </Badge>
  );
}
