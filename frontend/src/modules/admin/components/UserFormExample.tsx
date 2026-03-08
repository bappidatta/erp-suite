import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Label } from "@app/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@app/components/ui/select";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@app/components/ui/card";
import { Alert, AlertDescription } from "@app/components/ui/alert";
import { AlertCircle } from "lucide-react";

// Zod validation schema
const userSchema = z.object({
  email: z.string().email("Invalid email address"),
  firstName: z.string().min(2, "First name must be at least 2 characters"),
  lastName: z.string().min(2, "Last name must be at least 2 characters"),
  role: z.string().min(1, "Please select a role"),
});

type UserFormData = z.infer<typeof userSchema>;

export function UserFormExample() {
  const { register, handleSubmit, formState: { errors }, watch, reset } = useForm<UserFormData>({
    resolver: zodResolver(userSchema),
  });

  const onSubmit = (data: UserFormData) => {
    console.log("Form Data:", data);
    // Here you would call your API
    // await apiFetch("/api/v1/users", { method: "POST", body: data })
    alert("User would be created:\n" + JSON.stringify(data, null, 2));
    reset();
  };

  const role = watch("role");

  return (
    <Card className="w-full max-w-md">
      <CardHeader>
        <CardTitle>Create New User</CardTitle>
        <CardDescription>Add a new user to your system</CardDescription>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          {/* Email Field */}
          <div className="space-y-2">
            <Label htmlFor="email">Email</Label>
            <Input
              id="email"
              type="email"
              placeholder="user@example.com"
              {...register("email")}
            />
            {errors.email && (
              <p className="text-xs text-red-600">{errors.email.message}</p>
            )}
          </div>

          {/* First Name Field */}
          <div className="space-y-2">
            <Label htmlFor="firstName">First Name</Label>
            <Input
              id="firstName"
              placeholder="John"
              {...register("firstName")}
            />
            {errors.firstName && (
              <p className="text-xs text-red-600">{errors.firstName.message}</p>
            )}
          </div>

          {/* Last Name Field */}
          <div className="space-y-2">
            <Label htmlFor="lastName">Last Name</Label>
            <Input
              id="lastName"
              placeholder="Doe"
              {...register("lastName")}
            />
            {errors.lastName && (
              <p className="text-xs text-red-600">{errors.lastName.message}</p>
            )}
          </div>

          {/* Role Select */}
          <div className="space-y-2">
            <Label htmlFor="role">Role</Label>
            <Select value={role} onValueChange={(value) => register("role").onChange({ target: { value } })}>
              <SelectTrigger id="role">
                <SelectValue placeholder="Select a role" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="admin">Administrator</SelectItem>
                <SelectItem value="finance">Finance Manager</SelectItem>
                <SelectItem value="procurement">Procurement Manager</SelectItem>
                <SelectItem value="user">Standard User</SelectItem>
              </SelectContent>
            </Select>
            {errors.role && (
              <p className="text-xs text-red-600">{errors.role.message}</p>
            )}
          </div>

          {/* Info Alert */}
          {role === "admin" && (
            <Alert>
              <AlertCircle className="h-4 w-4" />
              <AlertDescription>
                Admin users have full access to all modules and settings.
              </AlertDescription>
            </Alert>
          )}

          {/* Submit Button */}
          <Button type="submit" className="w-full">
            Create User
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
