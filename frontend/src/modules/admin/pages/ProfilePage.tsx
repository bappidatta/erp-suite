import { Card, CardContent, CardHeader, CardTitle } from "@app/components/ui/card";
import { useAuth } from "@shared/auth/auth-context";
import { PageLayout, PageHeader } from "@shared/components";

export function ProfilePage() {
  const { auth } = useAuth();

  return (
    <PageLayout gap="6">
      <PageHeader title="Profile" description="Your account information" />

      <Card>
        <CardHeader>
          <CardTitle>User Details</CardTitle>
        </CardHeader>
        <CardContent className="space-y-2 text-sm">
          <p>
            <span className="font-medium">Full Name:</span> {auth?.user.fullName}
          </p>
          <p>
            <span className="font-medium">Email:</span> {auth?.user.email}
          </p>
          <p>
            <span className="font-medium">User Id:</span> {auth?.user.id}
          </p>
        </CardContent>
      </Card>
    </PageLayout>
  );
}
