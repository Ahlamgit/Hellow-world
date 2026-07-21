import axios from 'axios';

export function getApiErrorMessage(error: unknown, fallback = 'An error occurred'): string {
  if (axios.isAxiosError(error)) {
    if (!error.response) {
      return 'Cannot reach the API. Make sure the backend is running on http://localhost:5000 and restart the web dev server.';
    }
    const data = error.response?.data as { message?: string; errors?: string[] } | undefined;
    if (data?.errors?.length) return data.errors.join(', ');
    if (data?.message) return data.message;
  }
  if (error instanceof Error && error.message) return error.message;
  return fallback;
}
