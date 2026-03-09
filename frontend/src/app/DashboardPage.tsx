import { useAuth } from "@shared/auth/auth-context";
import { PageLayout } from "@shared/components";
import { Card, CardContent, CardHeader, CardTitle } from "@app/components/ui/card";
import { Badge } from "@app/components/ui/badge";
import {
  ArrowUpRight,
  ArrowDownRight,
  DollarSign,
  Users,
  FileText,
  Activity,
  ShoppingCart,
  TrendingUp,
  Clock,
  CheckCircle,
} from "lucide-react";

const metrics = [
  {
    title: "Total Revenue",
    value: "$45,231",
    change: "+12.5%",
    trend: "up" as const,
    detail: "vs last month",
    icon: DollarSign,
    color: "text-primary bg-primary/10",
  },
  {
    title: "Active Users",
    value: "1,234",
    change: "+8.2%",
    trend: "up" as const,
    detail: "vs last week",
    icon: Users,
    color: "text-chart-2 bg-chart-2/10",
  },
  {
    title: "Pending Invoices",
    value: "23",
    change: "$12,500",
    trend: "down" as const,
    detail: "total outstanding",
    icon: FileText,
    color: "text-chart-4 bg-chart-4/10",
  },
  {
    title: "Purchase Orders",
    value: "89",
    change: "+15%",
    trend: "up" as const,
    detail: "this quarter",
    icon: ShoppingCart,
    color: "text-chart-5 bg-chart-5/10",
  },
];

const recentActivity = [
  {
    action: "Invoice #INV-2024-089 approved",
    module: "Finance",
    time: "2 min ago",
    status: "success" as const,
  },
  {
    action: "New purchase order created",
    module: "Procurement",
    time: "15 min ago",
    status: "info" as const,
  },
  {
    action: "User John Doe updated profile",
    module: "Admin",
    time: "1 hour ago",
    status: "info" as const,
  },
  {
    action: "Stock alert: Widget A below threshold",
    module: "Inventory",
    time: "2 hours ago",
    status: "warning" as const,
  },
  {
    action: "Monthly report generated",
    module: "Reporting",
    time: "3 hours ago",
    status: "success" as const,
  },
];

const quickStats = [
  { label: "Open Tasks", value: "12", icon: Clock },
  { label: "Completed Today", value: "8", icon: CheckCircle },
  { label: "Revenue Growth", value: "23%", icon: TrendingUp },
  { label: "System Uptime", value: "99.9%", icon: Activity },
];

export function DashboardPage() {
  const { auth } = useAuth();

  const greeting = (() => {
    const hour = new Date().getHours();
    if (hour < 12) return "Good morning";
    if (hour < 17) return "Good afternoon";
    return "Good evening";
  })();

  return (
    <PageLayout gap="6" className="max-w-7xl mx-auto">
      {/* Welcome Section */}
      <div className="flex flex-col sm:flex-row sm:items-end justify-between gap-4">
        <div>
          <h1 className="text-2xl font-bold tracking-tight">
            {greeting}, {auth?.user.fullName?.split(" ")[0]}
          </h1>
          <p className="text-muted-foreground mt-1 text-sm">
            Here's an overview of your business today.
          </p>
        </div>
        <p className="text-sm text-muted-foreground">
          {new Date().toLocaleDateString("en-US", {
            weekday: "long",
            year: "numeric",
            month: "long",
            day: "numeric",
          })}
        </p>
      </div>

      {/* Metric Cards */}
      <div className="grid gap-4 grid-cols-1 sm:grid-cols-2 lg:grid-cols-4">
        {metrics.map((metric) => {
          const Icon = metric.icon;
          return (
            <Card key={metric.title} className="hover:shadow-md transition-shadow duration-200">
              <CardContent className="pt-5">
                <div className="flex items-start justify-between">
                  <div className="space-y-2">
                    <p className="text-sm text-muted-foreground">{metric.title}</p>
                    <p className="text-2xl font-bold tracking-tight">{metric.value}</p>
                    <div className="flex items-center gap-1.5">
                      {metric.trend === "up" ? (
                        <ArrowUpRight className="h-3.5 w-3.5 text-green-600" />
                      ) : (
                        <ArrowDownRight className="h-3.5 w-3.5 text-amber-600" />
                      )}
                      <span
                        className={`text-xs font-medium ${
                          metric.trend === "up" ? "text-green-600" : "text-amber-600"
                        }`}
                      >
                        {metric.change}
                      </span>
                      <span className="text-xs text-muted-foreground">{metric.detail}</span>
                    </div>
                  </div>
                  <div className={`p-2.5 rounded-xl ${metric.color}`}>
                    <Icon className="h-5 w-5" />
                  </div>
                </div>
              </CardContent>
            </Card>
          );
        })}
      </div>

      {/* Two-column layout */}
      <div className="grid gap-4 lg:grid-cols-5">
        {/* Recent Activity */}
        <Card className="lg:col-span-3">
          <CardHeader>
            <CardTitle className="text-base">Recent Activity</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-1">
              {recentActivity.map((item, i) => (
                <div
                  key={i}
                  className="flex items-center justify-between gap-4 py-3 border-b last:border-0"
                >
                  <div className="flex items-center gap-3 min-w-0">
                    <div
                      className={`h-2 w-2 rounded-full shrink-0 ${
                        item.status === "success"
                          ? "bg-green-500"
                          : item.status === "warning"
                          ? "bg-amber-500"
                          : "bg-primary"
                      }`}
                    />
                    <div className="min-w-0">
                      <p className="text-sm truncate">{item.action}</p>
                      <p className="text-xs text-muted-foreground">{item.time}</p>
                    </div>
                  </div>
                  <Badge variant="secondary" className="shrink-0 text-xs">
                    {item.module}
                  </Badge>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>

        {/* Quick Stats */}
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle className="text-base">Quick Stats</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-2 gap-4">
              {quickStats.map((stat) => {
                const Icon = stat.icon;
                return (
                  <div
                    key={stat.label}
                    className="flex flex-col items-center gap-2 p-4 rounded-xl bg-muted/50 text-center"
                  >
                    <Icon className="h-5 w-5 text-muted-foreground" />
                    <p className="text-xl font-bold">{stat.value}</p>
                    <p className="text-xs text-muted-foreground">{stat.label}</p>
                  </div>
                );
              })}
            </div>
          </CardContent>
        </Card>
      </div>
    </PageLayout>
  );
}
