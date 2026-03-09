import { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import {
  LayoutDashboard,
  Users,
  FileText,
  ShoppingCart,
  Settings,
  LogOut,
  ChevronDown,
  Menu,
  Package,
} from "lucide-react";
import { cn } from "@app/lib/utils";
import { Button } from "@app/components/ui/button";
import { ScrollArea } from "@app/components/ui/scroll-area";
import { Sheet, SheetContent, SheetTrigger } from "@app/components/ui/sheet";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@app/components/ui/collapsible";
import { useAuth } from "@shared/auth/auth-context";

const navItems = [
  {
    label: "Dashboard",
    href: "/dashboard",
    icon: LayoutDashboard,
  },
  {
    label: "Admin",
    icon: Users,
    submenu: [
      { label: "Overview", href: "/admin" },
      { label: "Users", href: "/admin/users" },
      { label: "Roles", href: "/admin/roles" },
      { label: "Organization", href: "/admin/organization" },
      { label: "Audit Log", href: "/admin/audit-log" },
    ],
  },
  {
    label: "Finance",
    icon: FileText,
    submenu: [
      { label: "Invoices", href: "/finance/invoices" },
      { label: "Expenses", href: "/finance/expenses" },
      { label: "Reports", href: "/finance/reports" },
    ],
  },
  {
    label: "Inventory",
    icon: Package,
    submenu: [
      { label: "Products", href: "/inventory/products" },
      { label: "Warehouses", href: "/inventory/warehouses" },
      { label: "Stock", href: "/inventory/stock" },
    ],
  },
  {
    label: "Procurement",
    icon: ShoppingCart,
    submenu: [
      { label: "Purchase Orders", href: "/procurement/orders" },
      { label: "Vendors", href: "/procurement/vendors" },
      { label: "Requisitions", href: "/procurement/requisitions" },
    ],
  },
];

interface NavItemProps {
  item: (typeof navItems)[0];
  currentPath: string;
}

function NavItem({ item, currentPath }: NavItemProps) {
  const Icon = item.icon;
  const isActive = item.href ? currentPath === item.href : false;

  if (!item.submenu) {
    return (
      <Link
        to={item.href || "#"}
        className={cn(
          "flex items-center gap-3 px-3 py-2.5 rounded-lg text-sm font-medium transition-all duration-200",
          isActive
            ? "bg-sidebar-primary text-sidebar-primary-foreground shadow-sm"
            : "text-sidebar-foreground/70 hover:text-sidebar-foreground hover:bg-sidebar-accent"
        )}
      >
        <Icon className="h-[18px] w-[18px] shrink-0" />
        <span>{item.label}</span>
      </Link>
    );
  }

  const isGroupActive = item.submenu.some((sub) => currentPath === sub.href);

  return (
    <Collapsible defaultOpen={isGroupActive}>
      <CollapsibleTrigger
        className={cn(
          "group w-full flex items-center justify-between px-3 py-2.5 rounded-lg text-sm font-medium transition-all duration-200 cursor-pointer",
          isGroupActive
            ? "text-sidebar-foreground bg-sidebar-accent"
            : "text-sidebar-foreground/70 hover:text-sidebar-foreground hover:bg-sidebar-accent"
        )}
      >
        <div className="flex items-center gap-3">
          <Icon className="h-[18px] w-[18px] shrink-0" />
          <span>{item.label}</span>
        </div>
        <ChevronDown className="h-4 w-4 transition-transform duration-200 group-data-[panel-open]:rotate-180" />
      </CollapsibleTrigger>
      <CollapsibleContent className="mt-1 ml-3 pl-3 border-l border-sidebar-border">
        <div className="space-y-0.5 py-1">
          {item.submenu.map((subitem) => {
            const isSubActive = currentPath === subitem.href;
            return (
              <Link
                key={subitem.href}
                to={subitem.href}
                className={cn(
                  "block px-3 py-2 rounded-md text-[13px] transition-all duration-200",
                  isSubActive
                    ? "text-sidebar-primary-foreground bg-sidebar-primary font-medium shadow-sm"
                    : "text-sidebar-foreground/60 hover:text-sidebar-foreground hover:bg-sidebar-accent"
                )}
              >
                {subitem.label}
              </Link>
            );
          })}
        </div>
      </CollapsibleContent>
    </Collapsible>
  );
}

function SidebarContent({ onLogout }: { onLogout: () => void }) {
  const location = useLocation();

  return (
    <>
      {/* Logo */}
      <div className="px-5 py-6">
        <div className="flex items-center gap-2.5">
          <div className="h-8 w-8 rounded-lg bg-sidebar-primary flex items-center justify-center">
            <span className="text-sidebar-primary-foreground font-bold text-sm">E</span>
          </div>
          <div>
            <h1 className="text-[15px] font-semibold text-sidebar-foreground tracking-tight">
              ERP Suite
            </h1>
            <p className="text-[11px] text-sidebar-foreground/50 leading-none">
              Management System
            </p>
          </div>
        </div>
      </div>

      {/* Divider */}
      <div className="mx-4 h-px bg-sidebar-border" />

      {/* Navigation */}
      <ScrollArea className="flex-1 px-3 py-4">
        <nav className="space-y-1">
          {navItems.map((item) => (
            <NavItem
              key={item.label}
              item={item}
              currentPath={location.pathname}
            />
          ))}
        </nav>
      </ScrollArea>

      {/* Bottom Actions */}
      <div className="mx-4 h-px bg-sidebar-border" />
      <div className="p-3 space-y-0.5">
        <Button
          variant="ghost"
          className="w-full justify-start gap-3 h-10 px-3 text-sm font-medium text-sidebar-foreground/70 hover:text-sidebar-foreground hover:bg-sidebar-accent"
        >
          <Settings className="h-[18px] w-[18px]" />
          <span>Settings</span>
        </Button>
        <Button
          variant="ghost"
          className="w-full justify-start gap-3 h-10 px-3 text-sm font-medium text-red-400 hover:text-red-300 hover:bg-red-500/10"
          onClick={onLogout}
        >
          <LogOut className="h-[18px] w-[18px]" />
          <span>Logout</span>
        </Button>
      </div>
    </>
  );
}

export function Sidebar({ className }: { className?: string }) {
  const { logout } = useAuth();

  return (
    <div
      className={cn(
        "w-[260px] bg-sidebar flex flex-col h-screen border-r border-sidebar-border",
        className
      )}
    >
      <SidebarContent onLogout={logout} />
    </div>
  );
}

export function MobileSidebar() {
  const [open, setOpen] = useState(false);
  const { logout } = useAuth();

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger
        className="lg:hidden"
        render={<Button variant="ghost" size="icon" />}
      >
        <Menu className="h-5 w-5" />
        <span className="sr-only">Open menu</span>
      </SheetTrigger>
      <SheetContent side="left" className="w-[260px] p-0 bg-sidebar border-sidebar-border">
        <div className="h-full flex flex-col">
          <SidebarContent
            onLogout={() => {
              logout();
              setOpen(false);
            }}
          />
        </div>
      </SheetContent>
    </Sheet>
  );
}
