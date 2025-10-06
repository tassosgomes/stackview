import { render, screen } from '@testing-library/react'
import { BrowserRouter } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { describe, it, expect, vi } from 'vitest'
import { DashboardPage } from '../pages/dashboard'


// Mock the stacks service
vi.mock('../services/stacks', () => ({
  stacksApi: {
    getMyStacks: vi.fn().mockResolvedValue({
      items: [],
      totalCount: 0,
      page: 1,
      pageSize: 10,
      totalPages: 0
    })
  }
}))

// Mock useAuth hook
vi.mock('../features/auth/use-auth', () => ({
  useAuth: () => ({
    user: { id: '1', email: 'test@test.com', name: 'Test User' },
    isAuthenticated: true,
    isLoading: false,
    logout: vi.fn()
  })
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

describe('DashboardPage', () => {
  it('renders dashboard with user welcome message', () => {
    render(
      <TestWrapper>
        <DashboardPage />
      </TestWrapper>
    )

    expect(screen.getByText(/bem-vindo, test user/i)).toBeInTheDocument()
    expect(screen.getByText(/meus stacks/i)).toBeInTheDocument()
    expect(screen.getByText(/gerenciar tokens mcp/i)).toBeInTheDocument() // Link to profile
  })

  it('shows empty state when user has no stacks', () => {
    render(
      <TestWrapper>
        <DashboardPage />
      </TestWrapper>
    )

    expect(screen.getByText(/você ainda não criou nenhum stack/i)).toBeInTheDocument()
    expect(screen.getByText(/carregando stacks/i)).toBeInTheDocument()
  })
})