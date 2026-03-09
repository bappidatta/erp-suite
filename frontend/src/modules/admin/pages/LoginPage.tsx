import { useState } from "react";
import { Link } from "react-router-dom";
import { Alert, AlertDescription } from "@app/components/ui/alert";
import { Button } from "@app/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@app/components/ui/card";
import { Input } from "@app/components/ui/input";
import { Label } from "@app/components/ui/label";
import { useLoginMutation } from "@modules/admin/hooks/useLoginMutation";
import { AlertTriangle, Lock, Mail, ArrowRight, Shield } from "lucide-react";

export function LoginPage() {
  const loginMutation = useLoginMutation();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    loginMutation.mutate({ email, password });
  }

  return (
    <main className="min-h-screen grid lg:grid-cols-2">
      {/* Left Side - Branding */}
      <div className="hidden lg:flex flex-col justify-between p-10 bg-gradient-to-br from-primary via-chart-1 to-chart-5 text-primary-foreground relative overflow-hidden">
        {/* Decorative elements */}
        <div className="absolute top-20 right-20 w-64 h-64 bg-white/5 rounded-full blur-3xl" />
        <div className="absolute bottom-20 left-20 w-96 h-96 bg-white/5 rounded-full blur-3xl" />
        
        {/* Logo & Brand */}
        <div className="relative z-10">
          <div className="flex items-center gap-3 mb-8">
            <div className="h-12 w-12 rounded-xl bg-white/10 backdrop-blur-sm flex items-center justify-center border border-white/20">
              <span className="text-2xl font-bold">E</span>
            </div>
            <div>
              <h1 className="text-2xl font-bold tracking-tight">ERP Suite</h1>
              <p className="text-sm text-primary-foreground/70">Management System</p>
            </div>
          </div>
          
          <blockquote className="space-y-2 max-w-md">
            <p className="text-lg font-medium leading-relaxed">
              "Streamline your business operations with our comprehensive ERP solution. 
              Manage everything from finance to procurement in one powerful platform."
            </p>
          </blockquote>
        </div>

        {/* Features */}
        <div className="relative z-10 space-y-4 max-w-md">
          <div className="flex items-start gap-3">
            <div className="mt-1 h-8 w-8 rounded-lg bg-white/10 backdrop-blur-sm flex items-center justify-center shrink-0">
              <Shield className="h-4 w-4" />
            </div>
            <div>
              <h3 className="font-semibold mb-1">Secure & Reliable</h3>
              <p className="text-sm text-primary-foreground/70">
                Enterprise-grade security with role-based access control
              </p>
            </div>
          </div>
          
          <div className="flex items-start gap-3">
            <div className="mt-1 h-8 w-8 rounded-lg bg-white/10 backdrop-blur-sm flex items-center justify-center shrink-0">
              <svg className="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 10V3L4 14h7v7l9-11h-7z" />
              </svg>
            </div>
            <div>
              <h3 className="font-semibold mb-1">Lightning Fast</h3>
              <p className="text-sm text-primary-foreground/70">
                Modern architecture built for performance and scalability
              </p>
            </div>
          </div>
        </div>

        {/* Footer */}
        <div className="relative z-10 text-sm text-primary-foreground/60">
          © 2026 ERP Suite. All rights reserved.
        </div>
      </div>

      {/* Right Side - Login Form */}
      <div className="flex items-center justify-center p-8 bg-background">
        <div className="w-full max-w-md space-y-8">
          {/* Mobile Logo */}
          <div className="lg:hidden flex items-center justify-center gap-2 mb-8">
            <div className="h-10 w-10 rounded-xl bg-primary flex items-center justify-center">
              <span className="text-lg font-bold text-primary-foreground">E</span>
            </div>
            <div>
              <h1 className="text-xl font-bold">ERP Suite</h1>
              <p className="text-xs text-muted-foreground">Management System</p>
            </div>
          </div>

          {/* Header */}
          <div className="text-center space-y-2">
            <h2 className="text-3xl font-bold tracking-tight">Welcome back</h2>
            <p className="text-muted-foreground">
              Enter your credentials to access your account
            </p>
          </div>

          {/* Form Card */}
          <Card className="border-2">
            <CardContent className="pt-6">
              <form onSubmit={handleSubmit} className="space-y-5">
                {/* Email Field */}
                <div className="space-y-2">
                  <Label htmlFor="email" className="text-sm font-medium">
                    Email address
                  </Label>
                  <div className="relative">
                    <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                      id="email"
                      type="email"
                      value={email}
                      onChange={(event) => setEmail(event.target.value)}
                      autoComplete="username"
                      placeholder="admin@erpsuite.local"
                      className="pl-10 h-11"
                      required
                    />
                  </div>
                </div>

                {/* Password Field */}
                <div className="space-y-2">
                  <div className="flex items-center justify-between">
                    <Label htmlFor="password" className="text-sm font-medium">
                      Password
                    </Label>
                    <button
                      type="button"
                      className="text-sm text-primary hover:underline"
                      onClick={() => alert("Password reset functionality coming soon")}
                    >
                      Forgot password?
                    </button>
                  </div>
                  <div className="relative">
                    <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                      id="password"
                      type="password"
                      value={password}
                      onChange={(event) => setPassword(event.target.value)}
                      autoComplete="current-password"
                      placeholder="Enter your password"
                      className="pl-10 h-11"
                      required
                    />
                  </div>
                </div>

                {/* Error Alert */}
                {loginMutation.error && (
                  <Alert variant="destructive" className="border-destructive/50">
                    <AlertTriangle className="h-4 w-4" />
                    <AlertDescription>
                      {loginMutation.error instanceof Error 
                        ? loginMutation.error.message 
                        : "Invalid credentials or API is not reachable."}
                    </AlertDescription>
                  </Alert>
                )}

                {/* Submit Button */}
                <Button
                  type="submit"
                  className="w-full h-11 text-base font-medium"
                  disabled={loginMutation.isPending}
                >
                  {loginMutation.isPending ? (
                    <>
                      <div className="h-4 w-4 mr-2 border-2 border-white/30 border-t-white rounded-full animate-spin" />
                      Signing in...
                    </>
                  ) : (
                    <>
                      Sign in
                      <ArrowRight className="ml-2 h-4 w-4" />
                    </>
                  )}
                </Button>
              </form>

              {/* Demo Credentials Notice */}
              <div className="mt-6 p-4 rounded-lg bg-muted/50 border">
                <p className="text-xs text-center text-muted-foreground">
                  <strong className="font-semibold text-foreground">Demo Credentials:</strong>
                  <br />
                  Email: admin@erpsuite.local • Password: Admin@123
                </p>
              </div>
            </CardContent>
          </Card>

          {/* Footer Links */}
          <p className="text-center text-sm text-muted-foreground">
            Don't have an account?{" "}
            <Link
              to="/register"
              className="text-primary font-medium hover:underline"
            >
              Create an account
            </Link>
          </p>
        </div>
      </div>
    </main>
  );
}
