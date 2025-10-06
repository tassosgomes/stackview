import { apiClient } from '@/lib/api-client'
import type { Technology } from '@/types/stack'

export const technologiesApi = {
  // Get all technologies
  getTechnologies: async (): Promise<Technology[]> => {
    const response = await apiClient.get<Technology[]>('/technologies')
    return response.data
  },

  // Search/suggest technologies
  suggestTechnologies: async (name: string): Promise<Technology[]> => {
    const response = await apiClient.post<Technology[]>('/technologies/suggest', { name })
    return response.data
  },

  // Create new technology (if user can add)
  createTechnology: async (data: { name: string; category: string }): Promise<Technology> => {
    const response = await apiClient.post<Technology>('/technologies', data)
    return response.data
  }
}