import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Button } from "@app/components/ui/button";
import { Input } from "@app/components/ui/input";
import { Label } from "@app/components/ui/label";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@app/components/ui/card";

// Zod validation schema
const userSchema = z.object({
  email: z.string().email("Invalid email address"),
  firstName: z.string().min(2, "First name must be at least 2 characters"),
  lastName: z.string().min(2, "Last name must be at least 2 characters"),
  role: z.string().min(1, "Please select a role"),
});

type UserFormData = z.infer<typeof userSchema>;

export function UserFormExample() {
  const { register, handleSubmit, formState: { errors }, reset } = useForm<UserFormData>({
    resolver: zodResolver(userSchema),
  });

  const onSubmit = (data: UserFormData) => {
    console.log("Form Data:", data);
    // Here you would call your API
    // await apiFetch("/api/v1/users", { method: "POST", body: data })
    alert("User would be created:\n" + JSON.stringify(data, null, 2));
    reset();
  };

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
            <select
              id="role"
              {...register("role")}
              className="w-full px-3 py-2 border border-input rounded-md bg-background"
            >
              <option value="">Select a role</option>
              <option value="admin">Administrator</option>
              <option value="finance">Finance Manager</option>
              <option value="procurement">Procurement Manager</option>
              <option value="user">Standard User</option>
            </select>
            {errors.role && (
              <p className="text-xs text-red-600">{errors.role.message}</p>
            )}
          </div>

          {/* Submit Button */}
          <Button type="submit" className="w-full">
            Create User
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
