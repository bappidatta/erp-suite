import { useState } from "react";
import { Link, useLocation } from "react-router-dom";
import {
  LayoutDashboard,
  Users,
  FileText,
  Settings,
  LogOut,
  ChevronDown,
  Menu,
  X,
} from "lucide-react";
import { cn } from "@app/lib/utils";
import { Button } from "@app/components/ui/button";
import { ScrollArea } from "@app/components/ui/scroll-area";
import { Separator } from "@app/components/ui/separator";
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
      { label: "Users", href: "/admin/users" },
      { label: "Roles", href: "/admin/roles" },
      { label: "Companies", href: "/admin/companies" },
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
    label: "Procurement",
    icon: FileText,
    submenu: [
      { label: "Purchase Orders", href: "/procurement/orders" },
      { label: "Vendors", href: "/procurement/vendors" },
      { label: "Requisitions", href: "/procurement/requisitions" },
    ],
  },
];

interface NavItemProps {
  item: (typeof navItems)[0];
  isActive: boolean;
}

function NavItem({ item, isActive }: NavItemProps) {
  const Icon = item.icon;

  if (!item.submenu) {
    return (
      <Link
        to={item.href || "#"}
        className={cn(
          "flex items-center gap-3 px-3 py-2 rounded-lg font-medium transition-colors",
          isActive
            ? "bg-primary text-primary-foreground"
            : "text-muted-foreground hover:text-foreground hover:bg-accent"
        )}
      >
        <Icon className="h-5 w-5" />
        <span>{item.label}</span>
      </Link>
    );
  }

  return (
    <Collapsible defaultOpen={isActive}>
      <CollapsibleTrigger className="w-full flex items-center justify-between px-3 py-2 rounded-lg font-medium text-muted-foreground hover:text-foreground hover:bg-accent transition-colors cursor-pointer">
        <div className="flex items-center gap-3">
          <Icon className="h-5 w-5" />
          <span>{item.label}</span>
        </div>
        <ChevronDown className="h-4 w-4 transition-transform" />
      </CollapsibleTrigger>
      <CollapsibleContent className="ml-4 mt-2 space-y-2">
        {item.submenu.map((subitem) => (
          <Link
            key={subitem.href}
            to={subitem.href}
            className={cn(
              "block px-3 py-2 rounded-lg text-sm transition-colors",
              isActive
                ? "bg-primary text-primary-foreground"
                : "text-muted-foreground hover:text-foreground hover:bg-accent"
            )}
          >
            {subitem.label}
          </Link>
        ))}
      </CollapsibleContent>
    </Collapsible>
  );
}

export function Sidebar({ className }: { className?: string }) {
  const location = useLocation();
  const { logout } = useAuth();

  return (
    <div className={cn("w-64 bg-card border-r flex flex-col h-screen", className)}>
      {/* Logo Section */}
      <div className="p-6 border-b">
        <h1 className="text-2xl font-bold bg-gradient-to-r from-primary to-blue-600 bg-clip-text text-transparent">
          ERP Suite
        </h1>
        <p className="text-xs text-muted-foreground mt-1">Management System</p>
      </div>

      {/* Navigation */}
      <ScrollArea className="flex-1 px-4 py-6">
        <nav className="space-y-2">
          {navItems.map((item) => (
            <NavItem
              key={item.label}
              item={item}
              isActive={
                item.submenu
                  ? item.submenu.some((sub) => location.pathname === sub.href)
                  : location.pathname === item.href
              }
            />
          ))}
        </nav>
      </ScrollArea>

      {/* Settings and Logout */}
      <div className="p-4 border-t space-y-2">
        <Button variant="ghost" className="w-full justify-start gap-3">
          <Settings className="h-5 w-5" />
          <span>Settings</span>
        </Button>
        <Button
          variant="ghost"
          className="w-full justify-start gap-3 text-red-600 hover:text-red-700 hover:bg-red-50"
          onClick={logout}
        >
          <LogOut className="h-5 w-5" />
          <span>Logout</span>
        </Button>
      </div>
    </div>
  );
}

export function MobileSidebar() {
  const [open, setOpen] = useState(false);
  const location = useLocation();
  const { logout } = useAuth();

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger className="lg:hidden">
        <Button variant="outline" size="icon">
          <Menu className="h-5 w-5" />
        </Button>
      </SheetTrigger>
      <SheetContent side="left" className="w-64 p-0">
        <div className="h-full flex flex-col bg-card">
          {/* Logo Section */}
          <div className="p-6 border-b">
            <h1 className="text-2xl font-bold bg-gradient-to-r from-primary to-blue-600 bg-clip-text text-transparent">
              ERP Suite
            </h1>
            <p className="text-xs text-muted-foreground mt-1">Management System</p>
          </div>

          {/* Navigation */}
          <ScrollArea className="flex-1 px-4 py-6">
            <nav className="space-y-2">
              {navItems.map((item) => (
                <NavItem
                  key={item.label}
                  item={item}
                  isActive={
                    item.submenu
                      ? item.submenu.some((sub) => location.pathname === sub.href)
                      : location.pathname === item.href
                  }
                />
              ))}
            </nav>
          </ScrollArea>

          {/* Settings and Logout */}
          <div className="p-4 border-t space-y-2">
            <Button variant="ghost" className="w-full justify-start gap-3">
              <Settings className="h-5 w-5" />
              <span>Settings</span>
            </Button>
            <Button
              variant="ghost"
              className="w-full justify-start gap-3 text-red-600 hover:text-red-700 hover:bg-red-50"
              onClick={() => {
                logout();
                setOpen(false);
              }}
            >
              <LogOut className="h-5 w-5" />
              <span>Logout</span>
            </Button>
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
}
