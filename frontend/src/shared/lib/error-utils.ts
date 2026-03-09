/**
 * Parses API error responses and returns user-friendly error messages.
 * 
 * Handles various error formats including:
 * - Validation errors (errors object with field-specific messages)
 * - Problem details (title field)
 * - Generic Error objects
 * 
 * @param error - The error object to parse
 * @returns A user-friendly error message string
 */
export function parseApiError(error: unknown): string {
  if (!error) return "An unexpected error occurred.";
  
  if (error instanceof Error) {
    try {
      // Try to parse JSON error response
      const parsed = JSON.parse(error.message);
      
      // Check for validation errors
      if (parsed.errors && typeof parsed.errors === "object") {
        const errorMessages: string[] = [];
        for (const field in parsed.errors) {
          const fieldErrors = parsed.errors[field];
          if (Array.isArray(fieldErrors)) {
            errorMessages.push(...fieldErrors);
          }
        }
        if (errorMessages.length > 0) {
          return errorMessages.join(" ");
        }
      }
      
      // Check for title or message fields
      if (parsed.title) {
        return parsed.title;
      }
    } catch {
      // If parsing fails, return the original message
      return error.message;
    }
  }
  
  return "An unexpected error occurred. Please try again.";
}
