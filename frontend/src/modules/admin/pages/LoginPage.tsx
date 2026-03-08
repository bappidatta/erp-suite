import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Alert, AlertDescription, AlertTitle } from "@app/components/ui/alert";
import { Button } from "@app/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@app/components/ui/card";
import { Input } from "@app/components/ui/input";
import { Label } from "@app/components/ui/label";
import { useAuth } from "@shared/auth/auth-context";
import { AlertTriangle } from "lucide-react";

export function LoginPage() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const [email, setEmail] = useState("admin@erpsuite.local");
  const [password, setPassword] = useState("Admin@123");
  const [error, setError] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setIsSubmitting(true);

    try {
      await login(email, password);
      navigate("/dashboard", { replace: true });
    } catch {
      setError("Invalid credentials or API is not reachable.");
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="min-h-screen flex items-center justify-center bg-muted/40 px-4 py-8">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle>ERP Suite Login</CardTitle>
          <CardDescription>Sign in with your admin account to access the MVP dashboard.</CardDescription>
        </CardHeader>

        <CardContent>
          <form onSubmit={handleSubmit}>
            <Label htmlFor="email">Email</Label>
            <Input
              id="email"
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              autoComplete="username"
              placeholder="admin@erpsuite.local"
              required
            />

            <Label htmlFor="password">Password</Label>
            <Input
              id="password"
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              autoComplete="current-password"
              placeholder="Enter your password"
              required
            />

            {error && (
              <Alert variant="destructive">
                <AlertTriangle size={16} />
                <AlertTitle>Sign in failed</AlertTitle>
                <AlertDescription>{error}</AlertDescription>
              </Alert>
            )}

            <Button type="submit" className="w-full" disabled={isSubmitting}>
              {isSubmitting ? "Signing in..." : "Sign in"}
            </Button>
          </form>
        </CardContent>
      </Card>
    </main>
  );
}
