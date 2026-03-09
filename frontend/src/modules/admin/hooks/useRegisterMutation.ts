import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@shared/auth/auth-context";

interface RegisterCredentials {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
}

export function useRegisterMutation() {
  const navigate = useNavigate();
  const { register } = useAuth();

  return useMutation({
    mutationFn: async (credentials: RegisterCredentials) => {
      await register(
        credentials.email,
        credentials.password,
        credentials.firstName,
        credentials.lastName
      );
    },
    onSuccess: () => {
      navigate("/dashboard", { replace: true });
    }
  });
}
