import { useQuery } from "@tanstack/react-query";
import { apiFetch } from "@shared/api/client";
import { useAuth } from "@shared/auth/auth-context";

interface DashboardMetrics {
  totalRevenue: string;
  revenueChange: string;
  activeUsers: number;
  usersChange: string;
  pendingInvoices: number;
  invoicesTotal: string;
  purchaseOrders: number;
  ordersChange: string;
}

interface ActivityItem {
  action: string;
  module: string;
  time: string;
  status: "success" | "info" | "warning";
}

interface DashboardData {
  metrics: DashboardMetrics;
  recentActivity: ActivityItem[];
  quickStats: Array<{
    label: string;
    value: string;
  }>;
}

export function useDashboardData() {
  const { auth } = useAuth();

  return useQuery({
    queryKey: ["dashboard", "overview"],
    queryFn: async () => {
      return apiFetch<DashboardData>("/api/v1/dashboard", {}, auth?.token);
    },
    enabled: !!auth?.token,
    staleTime: 60_000, // Consider fresh for 1 minute
    refetchInterval: 300_000 // Auto-refetch every 5 minutes
  });
}
