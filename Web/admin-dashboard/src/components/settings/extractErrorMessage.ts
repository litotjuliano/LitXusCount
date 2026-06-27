interface ApiErrorResponse {
  response?: {
    status?: number;
    data?: {
      // Our custom format
      message?: string;
      // ASP.NET ValidationProblemDetails
      errors?: Record<string, string[]>;
      // ASP.NET ProblemDetails
      title?: string;
      detail?: string;
    };
  };
}

export function extractErrorMessage(error: unknown): string {
  const data = (error as ApiErrorResponse)?.response?.data;
  if (!data) return "Something went wrong. Please try again.";

  // Our custom { message } format
  if (typeof data.message === 'string' && data.message) return data.message;

  // ASP.NET ValidationProblemDetails — { errors: { fieldName: ["msg"] } }
  if (data.errors && typeof data.errors === 'object') {
    const lines = Object.entries(data.errors)
      .flatMap(([field, msgs]) =>
        (msgs as string[]).map(m => field === '' ? m : `${field}: ${m}`)
      );
    if (lines.length > 0) return lines.join('\n');
  }

  // ASP.NET ProblemDetails — { title, detail }
  if (typeof data.title === 'string') {
    return data.detail ? `${data.title} — ${data.detail}` : data.title;
  }

  return "Something went wrong. Please try again.";
}
