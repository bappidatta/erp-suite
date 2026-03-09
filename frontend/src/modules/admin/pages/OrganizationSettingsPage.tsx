import { useEffect, useState } from "react";
import { Save, Upload } from "lucide-react";
import { Card, CardContent, CardHeader, CardTitle } from "@app/components/ui/card";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { getOrganizationSettings, updateOrganizationSettings, uploadLogo } from "../api/adminApi";
import type { OrganizationSettings, UpdateOrganizationSettingsRequest } from "../types";
import { PageHeader, FormError, PageLayout, FormField, FormGrid } from "@shared/components";

const TIMEZONES = ["UTC", "America/New_York", "America/Chicago", "America/Denver", "America/Los_Angeles", "Europe/London", "Europe/Paris", "Asia/Kolkata", "Asia/Tokyo", "Australia/Sydney"];
const CURRENCIES = ["USD", "EUR", "GBP", "INR", "JPY", "AUD", "CAD", "CHF", "CNY", "SGD"];
const DATE_FORMATS = ["MM/DD/YYYY", "DD/MM/YYYY", "YYYY-MM-DD", "DD.MM.YYYY"];

export function OrganizationSettingsPage() {
  const [settings, setSettings] = useState<OrganizationSettings | null>(null);
  const [form, setForm] = useState<UpdateOrganizationSettingsRequest>({
    companyName: "",
    currency: "USD",
  });
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [uploading, setUploading] = useState(false);
  const [saved, setSaved] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    getOrganizationSettings()
      .then((data) => {
        setSettings(data);
        setForm({
          companyName: data.companyName,
          legalName: data.legalName,
          registrationNumber: data.registrationNumber,
          address: data.address,
          phone: data.phone,
          email: data.email,
          website: data.website,
          currency: data.currency,
          fiscalYearStart: data.fiscalYearStart,
          dateFormat: data.dateFormat,
          timeZone: data.timeZone,
        });
      })
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError("");
    setSaved(false);
    try {
      const updated = await updateOrganizationSettings(form);
      setSettings(updated);
      setSaved(true);
      setTimeout(() => setSaved(false), 3000);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to save settings.");
    } finally {
      setSaving(false);
    }
  };

  const handleLogoUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    setUploading(true);
    try {
      const result = await uploadLogo(file);
      setSettings((prev) => prev ? { ...prev, logoPath: result.logoPath } : prev);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to upload logo.");
    } finally {
      setUploading(false);
    }
  };

  const field = (key: keyof UpdateOrganizationSettingsRequest) => ({
    value: (form[key] as string) ?? "",
    onChange: (e: React.ChangeEvent<HTMLInputElement>) =>
      setForm((f) => ({ ...f, [key]: e.target.value || undefined })),
  });

  if (loading) {
    return (
      <PageLayout>
        <div className="h-8 w-48 animate-pulse rounded bg-muted" />
        <div className="h-64 animate-pulse rounded bg-muted" />
      </PageLayout>
    );
  }

  return (
    <PageLayout gap="6" className="max-w-2xl">
      <PageHeader title="Organization Settings" description="Configure your organization information" />

      <FormError error={error} />
      {saved && <div className="rounded-md bg-green-50 p-3 text-sm text-green-700 border border-green-200">Settings saved successfully.</div>}

      <form onSubmit={handleSubmit} className="space-y-6">
        {/* Company Info */}
        <Card>
          <CardHeader><CardTitle className="text-base">Company Information</CardTitle></CardHeader>
          <CardContent className="space-y-4">
            <FormField id="companyName" label="Company Name *">
              <Input id="companyName" required value={form.companyName} onChange={(e) => setForm((f) => ({ ...f, companyName: e.target.value }))} />
            </FormField>
            <FormGrid gap="4">
              <FormField id="legalName" label="Legal Name">
                <Input id="legalName" {...field("legalName")} />
              </FormField>
              <FormField id="registrationNumber" label="Registration Number">
                <Input id="registrationNumber" {...field("registrationNumber")} />
              </FormField>
            </FormGrid>
            <FormField id="address" label="Address">
              <Input id="address" {...field("address")} />
            </FormField>
            <FormGrid gap="4">
              <FormField id="phone" label="Phone">
                <Input id="phone" type="tel" {...field("phone")} />
              </FormField>
              <FormField id="email" label="Email">
                <Input id="email" type="email" {...field("email")} />
              </FormField>
            </FormGrid>
            <FormField id="website" label="Website">
              <Input id="website" type="url" {...field("website")} />
            </FormField>
          </CardContent>
        </Card>

        {/* Regional Settings */}
        <Card>
          <CardHeader><CardTitle className="text-base">Regional Settings</CardTitle></CardHeader>
          <CardContent className="space-y-4">
            <FormGrid gap="4">
              <FormField id="currency" label="Currency">
                <select
                  id="currency"
                  className="flex h-9 w-full rounded-md border border-input bg-background px-3 py-1 text-sm shadow-sm"
                  value={form.currency ?? "USD"}
                  onChange={(e) => setForm((f) => ({ ...f, currency: e.target.value }))}
                >
                  {CURRENCIES.map((c) => <option key={c} value={c}>{c}</option>)}
                </select>
              </FormField>
              <FormField id="dateFormat" label="Date Format">
                <select
                  id="dateFormat"
                  className="flex h-9 w-full rounded-md border border-input bg-background px-3 py-1 text-sm shadow-sm"
                  value={form.dateFormat ?? ""}
                  onChange={(e) => setForm((f) => ({ ...f, dateFormat: e.target.value || undefined }))}
                >
                  <option value="">Select format</option>
                  {DATE_FORMATS.map((f) => <option key={f} value={f}>{f}</option>)}
                </select>
              </FormField>
            </FormGrid>
            <FormGrid gap="4">
              <FormField id="timeZone" label="Time Zone">
                <select
                  id="timeZone"
                  className="flex h-9 w-full rounded-md border border-input bg-background px-3 py-1 text-sm shadow-sm"
                  value={form.timeZone ?? ""}
                  onChange={(e) => setForm((f) => ({ ...f, timeZone: e.target.value || undefined }))}
                >
                  <option value="">Select timezone</option>
                  {TIMEZONES.map((tz) => <option key={tz} value={tz}>{tz}</option>)}
                </select>
              </FormField>
              <FormField id="fiscalYearStart" label="Fiscal Year Start (MM-DD)">
                <Input id="fiscalYearStart" placeholder="01-01" {...field("fiscalYearStart")} />
              </FormField>
            </FormGrid>
          </CardContent>
        </Card>

        {/* Logo */}
        <Card>
          <CardHeader><CardTitle className="text-base">Company Logo</CardTitle></CardHeader>
          <CardContent className="space-y-4">
            {settings?.logoPath && (
              <img src={settings.logoPath} alt="Logo" className="h-16 object-contain" />
            )}
            <div>
              <FormField id="logo" label="Upload Logo (JPG, PNG, SVG — max 5MB)">
                <div className="flex items-center gap-3">
                  <label
                    htmlFor="logo"
                    className="flex cursor-pointer items-center gap-2 rounded-md border px-3 py-2 text-sm hover:bg-muted"
                  >
                    <Upload className="h-4 w-4" />
                    {uploading ? "Uploading…" : "Choose file"}
                    <input id="logo" type="file" accept="image/*" className="sr-only" onChange={handleLogoUpload} disabled={uploading} />
                  </label>
                </div>
              </FormField>
            </div>
          </CardContent>
        </Card>

        <div className="flex justify-end">
          <Button type="submit" disabled={saving}>
            <Save className="mr-2 h-4 w-4" />
            {saving ? "Saving…" : "Save Settings"}
          </Button>
        </div>
      </form>
    </PageLayout>
  );
}
