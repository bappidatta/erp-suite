import type { ReactNode } from "react";

interface PageLayoutProps {
  children: ReactNode;
  /** Vertical spacing between sections. Defaults to "4". */
  gap?: "4" | "6";
  className?: string;
}

export function PageLayout({ children, gap = "4", className }: PageLayoutProps) {
  const base = gap === "6" ? "space-y-6" : "space-y-4";
  return <div className={className ? `${base} ${className}` : base}>{children}</div>;
}
