interface FormErrorProps {
  error: string;
}

export function FormError({ error }: FormErrorProps) {
  if (!error) return null;
  return (
    <div className="rounded-md bg-destructive/10 p-3 text-sm text-destructive">
      {error}
    </div>
  );
}
