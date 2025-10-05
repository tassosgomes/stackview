import { QueryClient } from '@tanstack/react-query'

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes
      gcTime: 1000 * 60 * 30, // 30 minutes (previously cacheTime)
      retry: (failureCount, error: unknown) => {
        // Don't retry on 4xx errors
        const axiosError = error as { response?: { status?: number } }
        const status = axiosError?.response?.status
        if (status && status >= 400 && status < 500) {
          return false
        }
        return failureCount < 3
      },
    },
  },
})