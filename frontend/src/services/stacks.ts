import { apiClient } from '@/lib/api-client'
import type { 
  Stack, 
  StackSummary, 
  CreateStackRequest, 
  UpdateStackRequest, 
  PagedList 
} from '@/types/stack'

export const stacksApi = {
  // Get user's stacks
  getMyStacks: async (page = 1, pageSize = 10): Promise<PagedList<StackSummary>> => {
    const response = await apiClient.get<PagedList<StackSummary>>('/stacks/me', {
      params: { page, pageSize }
    })
    return response.data
  },

  // Get all public stacks
  getPublicStacks: async (
    page = 1, 
    pageSize = 10, 
    type?: string, 
    technology?: string
  ): Promise<PagedList<StackSummary>> => {
    const response = await apiClient.get<PagedList<StackSummary>>('/stacks', {
      params: { page, pageSize, type, technology }
    })
    return response.data
  },

  // Get stack by ID
  getStack: async (id: string): Promise<Stack> => {
    const response = await apiClient.get<Stack>(`/stacks/${id}`)
    return response.data
  },

  // Create new stack
  createStack: async (data: CreateStackRequest): Promise<Stack> => {
    const response = await apiClient.post<Stack>('/stacks', data)
    return response.data
  },

  // Update stack
  updateStack: async (id: string, data: UpdateStackRequest): Promise<Stack> => {
    const response = await apiClient.put<Stack>(`/stacks/${id}`, data)
    return response.data
  },

  // Delete stack
  deleteStack: async (id: string): Promise<void> => {
    await apiClient.delete(`/stacks/${id}`)
  }
}