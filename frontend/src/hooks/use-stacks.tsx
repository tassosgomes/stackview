import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { stacksApi } from '@/services/stacks'
import { technologiesApi } from '@/services/technologies'
import type { CreateStackRequest, UpdateStackRequest } from '@/types/stack'

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

export function useCreateStack() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: (data: CreateStackRequest) => stacksApi.createStack(data),
    onSuccess: () => {
      // Invalidate and refetch my stacks
      queryClient.invalidateQueries({ queryKey: ['stacks', 'my'] })
      queryClient.invalidateQueries({ queryKey: ['stacks', 'public'] })
    }
  })
}

export function useUpdateStack() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: ({ id, data }: { id: string; data: UpdateStackRequest }) => 
      stacksApi.updateStack(id, data),
    onSuccess: (_, { id }) => {
      // Invalidate specific stack and lists
      queryClient.invalidateQueries({ queryKey: ['stacks', id] })
      queryClient.invalidateQueries({ queryKey: ['stacks', 'my'] })
      queryClient.invalidateQueries({ queryKey: ['stacks', 'public'] })
    }
  })
}

export function useDeleteStack() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: (id: string) => stacksApi.deleteStack(id),
    onSuccess: () => {
      // Invalidate lists
      queryClient.invalidateQueries({ queryKey: ['stacks', 'my'] })
      queryClient.invalidateQueries({ queryKey: ['stacks', 'public'] })
    }
  })
}

export function useTechnologies() {
  return useQuery({
    queryKey: ['technologies'],
    queryFn: () => technologiesApi.getTechnologies()
  })
}

export function useSuggestTechnologies(name: string) {
  return useQuery({
    queryKey: ['technologies', 'suggest', name],
    queryFn: () => technologiesApi.suggestTechnologies(name),
    enabled: !!name && name.length >= 2
  })
}