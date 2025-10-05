import { useQuery } from '@tanstack/react-query'
import { stacksApi } from '@/services/stacks'

export function useMyStacks(page = 1, pageSize = 10) {
  return useQuery({
    queryKey: ['stacks', 'my', page, pageSize],
    queryFn: () => stacksApi.getMyStacks(page, pageSize)
  })
}

export function usePublicStacks(
  page = 1, 
  pageSize = 10, 
  type?: string, 
  technology?: string
) {
  return useQuery({
    queryKey: ['stacks', 'public', page, pageSize, type, technology],
    queryFn: () => stacksApi.getPublicStacks(page, pageSize, type, technology)
  })
}

export function useStack(id: string) {
  return useQuery({
    queryKey: ['stacks', id],
    queryFn: () => stacksApi.getStack(id),
    enabled: !!id
  })
}