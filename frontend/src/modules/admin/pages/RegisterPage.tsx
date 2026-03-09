import { useState } from "react";
import { Link } from "react-router-dom";
import { Alert, AlertDescription } from "@app/components/ui/alert";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { useRegisterMutation } from "@modules/admin/hooks/useRegisterMutation";
import { FormField, FormGrid } from "@shared/components";
import { AlertTriangle, Lock, Mail, ArrowRight, User } from "lucide-react";
import { parseApiError } from "@shared/lib/error-utils";
import { AuthLayout } from "@app/components/layout/AuthLayout";

export function RegisterPage() {
  const registerMutation = useRegisterMutation();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [passwordError, setPasswordError] = useState("");

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setPasswordError("");

    // Validate password match
    if (password !== confirmPassword) {
      setPasswordError("Passwords do not match");
      return;
    }

    // Validate password strength
    const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$/;
    if (!passwordRegex.test(password)) {
      setPasswordError(
        "Password must be at least 8 characters with uppercase, lowercase, number, and special character"
      );
      return;
    }

    registerMutation.mutate({ email, password, firstName, lastName });
  }

  return (
    <AuthLayout
      title="Create an account"
      subtitle="Enter your details to get started"
      quote="Join thousands of businesses streamlining their operations with our comprehensive ERP solution."
      mobileTitle="Management System"
      footerLinks={
        <p className="text-center text-sm text-muted-foreground">
          Already have an account?{" "}
          <Link
            to="/login"
            className="text-primary font-medium hover:underline"
          >
            Sign in
          </Link>
        </p>
      }
    >
      <form onSubmit={handleSubmit} className="space-y-4">
        {/* Name Fields */}
        <FormGrid gap="4">
          <FormField id="firstName" label="First name">
            <div className="relative">
              <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                id="firstName"
                type="text"
                value={firstName}
                onChange={(event) => setFirstName(event.target.value)}
                autoComplete="given-name"
                placeholder="John"
                className="pl-10 h-11"
                required
                maxLength={128}
              />
            </div>
          </FormField>

          <FormField id="lastName" label="Last name">
            <div className="relative">
              <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
              <Input
                id="lastName"
                type="text"
                value={lastName}
                onChange={(event) => setLastName(event.target.value)}
                autoComplete="family-name"
                placeholder="Doe"
                className="pl-10 h-11"
                required
                maxLength={128}
              />
            </div>
          </FormField>
        </FormGrid>

        {/* Email Field */}
        <FormField id="email" label="Email address">
          <div className="relative">
            <Mail className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              id="email"
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              autoComplete="email"
              placeholder="john.doe@example.com"
              className="pl-10 h-11"
              required
              maxLength={256}
            />
          </div>
        </FormField>

        {/* Password Field */}
        <FormField id="password" label="Password">
          <div className="relative">
            <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              id="password"
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              autoComplete="new-password"
              placeholder="Create a password"
              className="pl-10 h-11"
              required
            />
          </div>
        </FormField>

        {/* Confirm Password Field */}
        <FormField id="confirmPassword" label="Confirm password">
          <div className="relative">
            <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              id="confirmPassword"
              type="password"
              value={confirmPassword}
              onChange={(event) => setConfirmPassword(event.target.value)}
              autoComplete="new-password"
              placeholder="Confirm your password"
              className="pl-10 h-11"
              required
            />
          </div>
        </FormField>

        {/* Password Requirements */}
        <div className="p-3 rounded-lg bg-muted/50 border text-xs text-muted-foreground">
          Password must contain:
          <ul className="mt-1 ml-4 list-disc space-y-0.5">
            <li>At least 8 characters</li>
            <li>One uppercase and one lowercase letter</li>
            <li>One number</li>
            <li>One special character (@$!%*?&#)</li>
          </ul>
        </div>

        {/* Error Alerts */}
        {passwordError && (
          <Alert variant="destructive" className="border-destructive/50">
            <AlertTriangle className="h-4 w-4" />
            <AlertDescription>{passwordError}</AlertDescription>
          </Alert>
        )}

        {registerMutation.error && (
          <Alert variant="destructive" className="border-destructive/50">
            <AlertTriangle className="h-4 w-4" />
            <AlertDescription>
              {parseApiError(registerMutation.error)}
            </AlertDescription>
          </Alert>
        )}

        {/* Submit Button */}
        <Button
          type="submit"
          className="w-full h-11 text-base font-medium"
          disabled={registerMutation.isPending}
        >
          {registerMutation.isPending ? (
            <>
              <div className="h-4 w-4 mr-2 border-2 border-white/30 border-t-white rounded-full animate-spin" />
              Creating account...
            </>
          ) : (
            <>
              Create account
              <ArrowRight className="ml-2 h-4 w-4" />
            </>
          )}
        </Button>
      </form>
    </AuthLayout>
  );
}
