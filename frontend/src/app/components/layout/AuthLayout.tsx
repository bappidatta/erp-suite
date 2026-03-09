import { ReactNode } from "react";
import { Card, CardContent } from "@app/components/ui/card";
import { Shield } from "lucide-react";

interface AuthLayoutProps {
  title: string;
  subtitle: string;
  quote: string;
  children: ReactNode;
  footerLinks: ReactNode;
  mobileTitle: string;
}

export function AuthLayout({
  title,
  subtitle,
  quote,
  children,
  footerLinks,
  mobileTitle,
}: AuthLayoutProps) {
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
            <p className="text-lg font-medium leading-relaxed">{quote}</p>
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
                <path
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                  d="M13 10V3L4 14h7v7l9-11h-7z"
                />
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

      {/* Right Side - Form */}
      <div className="flex items-center justify-center p-8 bg-background">
        <div className="w-full max-w-md space-y-8">
          {/* Mobile Logo */}
          <div className="lg:hidden flex items-center justify-center gap-2 mb-8">
            <div className="h-10 w-10 rounded-xl bg-primary flex items-center justify-center">
              <span className="text-lg font-bold text-primary-foreground">E</span>
            </div>
            <div>
              <h1 className="text-xl font-bold">ERP Suite</h1>
              <p className="text-xs text-muted-foreground">{mobileTitle}</p>
            </div>
          </div>

          {/* Header */}
          <div className="text-center space-y-2">
            <h2 className="text-3xl font-bold tracking-tight">{title}</h2>
            <p className="text-muted-foreground">{subtitle}</p>
          </div>

          {/* Form Card */}
          <Card className="border-2">
            <CardContent className="pt-6">
              {children}
            </CardContent>
          </Card>

          {/* Footer Links */}
          {footerLinks}
        </div>
      </div>
    </main>
  );
}
