import { Card, CardContent, CardHeader, CardTitle } from "@app/components/ui/card";
import { useAuth } from "@shared/auth/auth-context";

export function ProfilePage() {
  const { auth } = useAuth();

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Profile</h1>
        <p className="text-muted-foreground">Your account information</p>
      </div>

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
    </div>
  );
}
