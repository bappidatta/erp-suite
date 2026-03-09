import type { ReactNode } from "react";

interface FormGridProps {
  cols?: 2 | 3;
  /** Gap between columns. Defaults to "3". */
  gap?: "3" | "4";
  children: ReactNode;
}

export function FormGrid({ cols = 2, gap = "3", children }: FormGridProps) {
  const colClass = cols === 3 ? "grid-cols-3" : "grid-cols-2";
  const gapClass = gap === "4" ? "gap-4" : "gap-3";
  return <div className={`grid ${colClass} ${gapClass}`}>{children}</div>;
}
