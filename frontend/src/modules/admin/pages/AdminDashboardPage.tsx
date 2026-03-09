import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Users, Shield, Activity, Clock, UserCheck, Settings } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@app/components/ui/card";
import { Badge } from "@app/components/ui/badge";
import { getDashboardStats, getAuditLogs } from "../api/adminApi";
import type { DashboardStats, AuditLog } from "../types";
import { PageLayout, PageHeader } from "@shared/components";

const quickLinks = [
  { label: "Users", href: "/admin/users", icon: Users, color: "text-blue-600" },
  { label: "Roles", href: "/admin/roles", icon: Shield, color: "text-purple-600" },
  { label: "Organization Settings", href: "/admin/organization", icon: Settings, color: "text-green-600" },
  { label: "Audit Log", href: "/admin/audit-log", icon: Activity, color: "text-orange-600" },
];

export function AdminDashboardPage() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [recentLogs, setRecentLogs] = useState<AuditLog[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      getDashboardStats().catch(() => null),
      getAuditLogs({ pageSize: "10" }).catch(() => ({ items: [] as AuditLog[], totalCount: 0, page: 1, pageSize: 10, totalPages: 0, hasPreviousPage: false, hasNextPage: false })),
    ]).then(([statsData, logsData]) => {
      setStats(statsData);
      setRecentLogs(logsData.items);
    }).finally(() => setLoading(false));
  }, []);

  const statCards = stats
    ? [
        { title: "Total Users", value: stats.totalUsers, icon: Users, sub: `${stats.activeUsers} active` },
        { title: "Roles", value: stats.totalRoles, icon: Shield, sub: `${stats.totalPermissions} permissions` },
        { title: "System Health", value: stats.systemHealth, icon: Activity, sub: "All systems operational" },
        { title: "Last Activity", value: stats.lastActivity ? new Date(stats.lastActivity).toLocaleDateString() : "—", icon: Clock, sub: stats.lastActivity ? new Date(stats.lastActivity).toLocaleTimeString() : "" },
      ]
    : [];

  return (
    <PageLayout gap="6">
      <PageHeader title="Admin Overview" description="System overview and administration" />

      {/* Stats */}
      <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {loading
          ? Array.from({ length: 4 }).map((_, i) => (
              <Card key={i}>
                <CardContent className="p-6">
                  <div className="h-16 animate-pulse rounded bg-muted" />
                </CardContent>
              </Card>
            ))
          : statCards.map((card) => (
              <Card key={card.title}>
                <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
                  <CardTitle className="text-sm font-medium">{card.title}</CardTitle>
                  <card.icon className="h-4 w-4 text-muted-foreground" />
                </CardHeader>
                <CardContent>
                  <div className="text-2xl font-bold">{card.value}</div>
                  <p className="text-xs text-muted-foreground">{card.sub}</p>
                </CardContent>
              </Card>
            ))}
      </div>

      {/* Quick Links */}
      <Card>
        <CardHeader>
          <CardTitle>Quick Links</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-4">
            {quickLinks.map((link) => (
              <Link
                key={link.href}
                to={link.href}
                className="flex flex-col items-center gap-2 rounded-lg border p-4 transition-colors hover:bg-muted"
              >
                <link.icon className={`h-6 w-6 ${link.color}`} />
                <span className="text-sm font-medium">{link.label}</span>
              </Link>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Recent Activity */}
      <Card>
        <CardHeader>
          <CardTitle>Recent Activity</CardTitle>
        </CardHeader>
        <CardContent>
          {recentLogs.length === 0 ? (
            <p className="text-sm text-muted-foreground">No recent activity.</p>
          ) : (
            <div className="space-y-2">
              {recentLogs.map((log) => (
                <div key={log.id} className="flex items-center justify-between rounded-md border p-3 text-sm">
                  <div className="flex items-center gap-3">
                    <UserCheck className="h-4 w-4 text-muted-foreground" />
                    <div>
                      <span className="font-medium">{log.action}</span>
                      <span className="text-muted-foreground"> · {log.module}</span>
                      {log.userName && <span className="text-muted-foreground"> by {log.userName}</span>}
                    </div>
                  </div>
                  <Badge variant="outline">{new Date(log.createdAt).toLocaleTimeString()}</Badge>
                </div>
              ))}
            </div>
          )}
        </CardContent>
      </Card>
    </PageLayout>
  );
}
