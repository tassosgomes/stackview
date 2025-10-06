import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { mcpTokensApi } from '@/services/mcp-tokens'
import { toast } from 'sonner'

const MCP_TOKENS_QUERY_KEY = ['mcp-tokens'] as const

export function useMcpTokens() {
  return useQuery({
    queryKey: MCP_TOKENS_QUERY_KEY,
    queryFn: mcpTokensApi.getMyTokens,
  })
}

export function useCreateMcpToken() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: mcpTokensApi.createToken,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MCP_TOKENS_QUERY_KEY })
      toast.success('Token criado com sucesso')
    },
    onError: (error: Error) => {
      toast.error('Falha ao criar token: ' + error.message)
    },
  })
}

export function useRevokeMcpToken() {
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: mcpTokensApi.revokeToken,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: MCP_TOKENS_QUERY_KEY })
      toast.success('Token revogado com sucesso')
    },
    onError: (error: Error) => {
      toast.error('Falha ao revogar token: ' + error.message)
    },
  })
}