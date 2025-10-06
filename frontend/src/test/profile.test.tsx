import { render, screen } from '@testing-library/react'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { describe, it, expect, vi } from 'vitest'
import { ProfilePage } from '../pages/profile'

// Mock the MCP tokens service
vi.mock('../services/mcp-tokens', () => ({
  mcpTokensApi: {
    getMyTokens: vi.fn().mockResolvedValue([])
  }
}))

// Mock useAuth hook
vi.mock('../features/auth/use-auth', () => ({
  useAuth: () => ({
    user: { id: '1', email: 'test@example.com', name: 'Test User' },
    isAuthenticated: true,
    isLoading: false,
    logout: vi.fn()
  })
}))

// Mock sonner toast
vi.mock('sonner', () => ({
  toast: {
    success: vi.fn(),
    error: vi.fn(),
  },
}))

const TestWrapper = ({ children }: { children: React.ReactNode }) => {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false }
    }
  })

  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        {children}
      </BrowserRouter>
    </QueryClientProvider>
  )
}

describe('ProfilePage', () => {
  it('renders profile page with user information', () => {
    render(
      <TestWrapper>
        <ProfilePage />
      </TestWrapper>
    )

    expect(screen.getByText('Perfil')).toBeInTheDocument()
    expect(screen.getByText('Test User')).toBeInTheDocument()
    expect(screen.getByText('test@example.com')).toBeInTheDocument()
    expect(screen.getByText('Informações Pessoais')).toBeInTheDocument()
  })

  it('renders MCP tokens section with loading state', () => {
    render(
      <TestWrapper>
        <ProfilePage />
      </TestWrapper>
    )

    expect(screen.getByText('Carregando tokens...')).toBeInTheDocument()
  })
})