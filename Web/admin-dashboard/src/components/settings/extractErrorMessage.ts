interface ApiErrorResponse {
  response?: {
    data?: {
      message?: string;
    };
  };
}

export function extractErrorMessage(error: unknown): string {
  const message = (error as ApiErrorResponse)?.response?.data?.message;
  return message ?? "Something went wrong. Please try again.";
}
