import { Bell, Search, User, LogOut, Settings } from "lucide-react";
import { Link } from "react-router-dom";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Separator } from "@app/components/ui/separator";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@app/components/ui/dropdown-menu";
import { useAuth } from "@shared/auth/auth-context";
import { MobileSidebar } from "./Sidebar";

export function Header() {
  const { auth, logout } = useAuth();
  const initials = auth?.user.fullName
    ?.split(" ")
    .map((n) => n[0])
    .join("")
    .slice(0, 2)
    .toUpperCase();

  return (
    <header className="bg-card/80 backdrop-blur-sm border-b sticky top-0 z-40">
      <div className="flex items-center justify-between px-4 md:px-6 h-16 gap-4">
        {/* Left — Mobile menu + Search */}
        <div className="flex items-center gap-3 flex-1">
          <MobileSidebar />

          <div className="hidden md:flex items-center relative flex-1 max-w-sm">
            <Search className="absolute left-3 h-4 w-4 text-muted-foreground pointer-events-none" />
            <Input
              type="text"
              placeholder="Search anything..."
              className="pl-9 h-9 bg-muted/50 border-0 focus-visible:ring-1 focus-visible:ring-ring"
            />
          </div>
        </div>

        {/* Right — Notifications + User */}
        <div className="flex items-center gap-2">
          <Button variant="ghost" size="icon" className="relative h-9 w-9">
            <Bell className="h-4 w-4" />
            <span className="absolute top-1.5 right-1.5 h-2 w-2 bg-primary rounded-full ring-2 ring-card" />
          </Button>

          <Separator orientation="vertical" className="h-6 mx-1" />

          {/* User Menu */}
          <DropdownMenu>
            <DropdownMenuTrigger render={<Button variant="ghost" className="gap-2.5 h-9 px-2" />}>
              <div className="h-8 w-8 rounded-full bg-gradient-to-br from-primary to-chart-5 flex items-center justify-center text-white text-xs font-semibold shrink-0">
                {initials}
              </div>
              <div className="hidden sm:block text-left">
                <p className="text-sm font-medium leading-none">{auth?.user.fullName}</p>
                <p className="text-[11px] text-muted-foreground mt-0.5">{auth?.user.email}</p>
              </div>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-56">
              <DropdownMenuGroup>
                <DropdownMenuLabel className="font-normal">
                  <div className="flex items-center gap-3">
                    <div className="h-10 w-10 rounded-full bg-gradient-to-br from-primary to-chart-5 flex items-center justify-center text-white text-sm font-semibold shrink-0">
                      {initials}
                    </div>
                    <div className="min-w-0">
                      <p className="text-sm font-medium truncate">{auth?.user.fullName}</p>
                      <p className="text-xs text-muted-foreground truncate">{auth?.user.email}</p>
                    </div>
                  </div>
                </DropdownMenuLabel>
              </DropdownMenuGroup>
              <DropdownMenuSeparator />
              <DropdownMenuItem>
                <Link to="/profile" className="flex items-center gap-2 w-full">
                  <User className="h-4 w-4" />
                  <span>Profile</span>
                </Link>
              </DropdownMenuItem>
              <DropdownMenuItem>
                <Settings className="h-4 w-4" />
                <span>Settings</span>
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onClick={logout}
                className="text-destructive focus:text-destructive"
              >
                <LogOut className="h-4 w-4" />
                <span>Logout</span>
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>
    </header>
  );
}
