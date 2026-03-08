import { Bell, Search, User, LogOut } from "lucide-react";
import { Link } from "react-router-dom";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Separator } from "@app/components/ui/separator";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@app/components/ui/dropdown-menu";
import { useAuth } from "@shared/auth/auth-context";
import { MobileSidebar } from "./Sidebar";

export function Header() {
  const { auth, logout } = useAuth();

  return (
    <header className="bg-card border-b sticky top-0 z-40">
      <div className="flex items-center justify-between px-6 py-4 gap-4">
        {/* Left side - Mobile menu + Search */}
        <div className="flex items-center gap-4 flex-1">
          <MobileSidebar />

          {/* Search Bar */}
          <div className="hidden md:flex items-center gap-2 flex-1 max-w-md">
            <Search className="h-5 w-5 text-muted-foreground" />
            <Input
              type="text"
              placeholder="Search..."
              className="border-0 bg-accent focus:bg-white focus-visible:ring-0"
            />
          </div>
        </div>

        {/* Right side - Notifications + User Menu */}
        <div className="flex items-center gap-4">
          {/* Notifications */}
          <Button variant="ghost" size="icon" className="relative">
            <Bell className="h-5 w-5" />
            <span className="absolute top-1 right-1 h-2 w-2 bg-red-500 rounded-full" />
          </Button>

          <Separator orientation="vertical" className="h-6" />

          {/* User Menu */}
          <DropdownMenu>
            <DropdownMenuTrigger>
              <Button variant="ghost" className="gap-2">
                <div className="w-8 h-8 rounded-full bg-gradient-to-br from-primary to-blue-600 flex items-center justify-center text-white text-sm font-bold">
                  {auth?.user.fullName?.charAt(0)}
                </div>
                <div className="hidden sm:block text-left">
                  <p className="text-sm font-medium">{auth?.user.fullName}</p>
                  <p className="text-xs text-muted-foreground">{auth?.user.email}</p>
                </div>
              </Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent align="end" className="w-56">
              <DropdownMenuLabel className="font-normal">
                <div className="flex items-center gap-2">
                  <div className="w-10 h-10 rounded-full bg-gradient-to-br from-primary to-blue-600 flex items-center justify-center text-white font-bold">
                    {auth?.user.fullName?.charAt(0)}
                  </div>
                  <div>
                    <p className="text-sm font-medium">{auth?.user.fullName}</p>
                    <p className="text-xs text-muted-foreground">{auth?.user.email}</p>
                  </div>
                </div>
              </DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem>
                <Link to="/profile" className="cursor-pointer flex items-center">
                  <User className="h-4 w-4 mr-2" />
                  <span>Profile</span>
                </Link>
              </DropdownMenuItem>
              <DropdownMenuItem>
                Settings
              </DropdownMenuItem>
              <DropdownMenuSeparator />
              <DropdownMenuItem
                onClick={logout}
                className="text-red-600 focus:text-red-700 focus:bg-red-50"
              >
                <LogOut className="h-4 w-4 mr-2" />
                <span>Logout</span>
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>
    </header>
  );
}
