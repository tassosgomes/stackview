import { apiClient } from '@/lib/api-client'
import type { McpTokenResponse, CreateMcpTokenResponse } from '@/types/mcp-tokens'

export const mcpTokensApi = {
  getMyTokens: async (): Promise<McpTokenResponse[]> => {
    const response = await apiClient.get<McpTokenResponse[]>('/users/me/mcp-tokens')
    return response.data
  },

  createToken: async (): Promise<CreateMcpTokenResponse> => {
    const response = await apiClient.post<CreateMcpTokenResponse>('/users/me/mcp-tokens')
    return response.data
  },

  revokeToken: async (tokenId: string): Promise<void> => {
    await apiClient.delete(`/users/me/mcp-tokens/${tokenId}`)
  }
}