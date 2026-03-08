import { useAuth } from "@shared/auth/auth-context";
import { DashboardGrid } from "@modules/admin/components/DashboardGrid";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@app/components/ui/card";
import { Badge } from "@app/components/ui/badge";
import { ArrowUpRight, ArrowDownRight } from "lucide-react";

export function DashboardPage() {
  const { auth } = useAuth();

  return (
    <div className="space-y-8">
      {/* Welcome Section */}
      <div>
        <h1 className="text-3xl font-bold">Welcome back, {auth?.user.fullName}! 👋</h1>
        <p className="text-muted-foreground mt-2">Here's what's happening with your business today.</p>
      </div>

      {/* Key Metrics */}
      <div className="grid gap-4 grid-cols-1 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Total Revenue</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">$45,231</div>
            <div className="flex items-center gap-1 mt-2 text-xs">
              <ArrowUpRight className="h-3 w-3 text-green-600" />
              <span className="text-green-600">+12.5% from last month</span>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Active Users</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">1,234</div>
            <div className="flex items-center gap-1 mt-2 text-xs">
              <ArrowUpRight className="h-3 w-3 text-green-600" />
              <span className="text-green-600">+8% from last week</span>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">Pending Invoices</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">$12,500</div>
            <div className="flex items-center gap-1 mt-2 text-xs">
              <ArrowDownRight className="h-3 w-3 text-orange-600" />
              <span className="text-orange-600">5 invoices due</span>
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="pb-2">
            <CardTitle className="text-sm font-medium text-muted-foreground">System Status</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">99.9%</div>
            <div className="flex items-center gap-1 mt-2 text-xs">
              <Badge className="bg-green-100 text-green-800">Operational</Badge>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Dashboard Grid with Tables and Examples */}
      <DashboardGrid />
    </div>
  );
}
