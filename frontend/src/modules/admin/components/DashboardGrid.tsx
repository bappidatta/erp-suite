import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@app/components/ui/card";
import { Badge } from "@app/components/ui/badge";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@app/components/ui/table";
import { Alert, AlertDescription, AlertTitle } from "@app/components/ui/alert";
import { AlertCircle } from "lucide-react";

export function DashboardGrid() {
  return (
    <div className="space-y-6">
      {/* Welcome Section */}
      <Alert>
        <AlertCircle className="h-4 w-4" />
        <AlertTitle>Welcome to ERP Suite</AlertTitle>
        <AlertDescription>
          You have successfully logged in. Start managing your business operations with the modules below.
        </AlertDescription>
      </Alert>

      {/* Stats Cards Grid */}
      <div className="grid gap-4 grid-cols-1 md:grid-cols-2 lg:grid-cols-4">
        <StatCard title="Total Users" value="24" status="active" />
        <StatCard title="Companies" value="12" status="success" />
        <StatCard title="Invoices Pending" value="8" status="warning" />
        <StatCard title="System Health" value="99.2%" status="success" />
      </div>

      {/* Recent Transactions Table */}
      <Card>
        <CardHeader>
          <CardTitle>Recent Transactions</CardTitle>
          <CardDescription>Latest activity in your system</CardDescription>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Type</TableHead>
                <TableHead>Amount</TableHead>
                <TableHead>Status</TableHead>
                <TableHead>Date</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow>
                <TableCell>#INV-001</TableCell>
                <TableCell>Invoice</TableCell>
                <TableCell>$2,500.00</TableCell>
                <TableCell><Badge>Paid</Badge></TableCell>
                <TableCell>Mar 8, 2026</TableCell>
              </TableRow>
              <TableRow>
                <TableCell>#PO-042</TableCell>
                <TableCell>Purchase Order</TableCell>
                <TableCell>$1,200.50</TableCell>
                <TableCell><Badge variant="secondary">Pending</Badge></TableCell>
                <TableCell>Mar 7, 2026</TableCell>
              </TableRow>
              <TableRow>
                <TableCell>#EXP-015</TableCell>
                <TableCell>Expense</TableCell>
                <TableCell>$450.00</TableCell>
                <TableCell><Badge variant="outline">Draft</Badge></TableCell>
                <TableCell>Mar 6, 2026</TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  );
}

interface StatCardProps {
  title: string;
  value: string;
  status: "active" | "success" | "warning" | "inactive";
}

function StatCard({ title, value, status }: StatCardProps) {
  const statusColors = {
    active: "bg-blue-100 text-blue-800",
    success: "bg-green-100 text-green-800",
    warning: "bg-yellow-100 text-yellow-800",
    inactive: "bg-gray-100 text-gray-800",
  };

  return (
    <Card>
      <CardHeader className="pb-2">
        <CardTitle className="text-sm font-medium text-muted-foreground">{title}</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="text-2xl font-bold">{value}</div>
        <Badge className={`mt-2 ${statusColors[status]}`}>
          {status.charAt(0).toUpperCase() + status.slice(1)}
        </Badge>
      </CardContent>
    </Card>
  );
}
