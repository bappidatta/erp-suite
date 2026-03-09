import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@shared/auth/auth-context";

interface LoginCredentials {
  email: string;
  password: string;
}

export function useLoginMutation() {
  const navigate = useNavigate();
  const { login } = useAuth();

  return useMutation({
    mutationFn: async (credentials: LoginCredentials) => {
      await login(credentials.email, credentials.password);
    },
    onSuccess: () => {
      navigate("/dashboard", { replace: true });
    }
  });
}
