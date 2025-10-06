import { describe, it, expect, vi, beforeEach } from 'vitest'
import { renderHook, waitFor } from '@testing-library/react'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useMcpTokens, useCreateMcpToken, useRevokeMcpToken } from '@/hooks/use-mcp-tokens'
import { mcpTokensApi } from '@/services/mcp-tokens'

// Mock the API service
vi.mock('@/services/mcp-tokens')
const mockedMcpTokensApi = vi.mocked(mcpTokensApi)

// Mock sonner toast
vi.mock('sonner', () => ({
  toast: {
    success: vi.fn(),
    error: vi.fn(),
  },
}))

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
    },
  })
  return ({ children }: { children: React.ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  )
}

describe('MCP Tokens Hooks', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  it('should fetch MCP tokens', async () => {
    const mockTokens = [
      {
        id: '123',
        createdAt: '2025-01-01T00:00:00Z',
        isRevoked: false,
      },
    ]
    mockedMcpTokensApi.getMyTokens.mockResolvedValue(mockTokens)

    const { result } = renderHook(() => useMcpTokens(), {
      wrapper: createWrapper(),
    })

    await waitFor(() => expect(result.current.isSuccess).toBe(true))
    expect(result.current.data).toEqual(mockTokens)
    expect(mockedMcpTokensApi.getMyTokens).toHaveBeenCalledOnce()
  })

  it('should create MCP token', async () => {
    const mockResponse = {
      id: '456',
      rawToken: 'mcp_token_12345',
      createdAt: '2025-01-01T00:00:00Z',
    }
    mockedMcpTokensApi.createToken.mockResolvedValue(mockResponse)

    const { result } = renderHook(() => useCreateMcpToken(), {
      wrapper: createWrapper(),
    })

    expect(result.current.mutate).toBeDefined()
    expect(mockedMcpTokensApi.createToken).not.toHaveBeenCalled()
  })

  it('should revoke MCP token', async () => {
    mockedMcpTokensApi.revokeToken.mockResolvedValue()

    const { result } = renderHook(() => useRevokeMcpToken(), {
      wrapper: createWrapper(),
    })

    expect(result.current.mutate).toBeDefined()
    expect(mockedMcpTokensApi.revokeToken).not.toHaveBeenCalled()
  })
})