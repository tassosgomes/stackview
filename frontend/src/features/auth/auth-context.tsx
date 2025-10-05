import { useState, useEffect } from 'react'
import type { ReactNode } from 'react'
import { authApi } from '@/services/auth'
import type { AuthContextType, User, LoginRequest, RegisterRequest } from '@/types/auth'
import { AuthContext } from './context'

interface AuthProviderProps {
  children: ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<User | null>(null)
  const [token, setToken] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    // Check for existing token on mount
    const storedToken = localStorage.getItem('authToken')
    const storedUser = localStorage.getItem('authUser')

    if (storedToken && storedUser) {
      setToken(storedToken)
      try {
        setUser(JSON.parse(storedUser))
      } catch (error) {
        console.error('Error parsing stored user:', error)
        localStorage.removeItem('authToken')
        localStorage.removeItem('authUser')
      }
    }

    setIsLoading(false)
  }, [])

  const login = async (credentials: LoginRequest) => {
    setIsLoading(true)
    try {
      const { user, token } = await authApi.login(credentials)
      
      // Store token and user data
      localStorage.setItem('authToken', token)
      localStorage.setItem('authUser', JSON.stringify(user))
      
      setToken(token)
      setUser(user)
    } finally {
      setIsLoading(false)
    }
  }

  const register = async (userData: RegisterRequest) => {
    setIsLoading(true)
    try {
      const { user, token } = await authApi.register(userData)
      
      // Store token and user data
      localStorage.setItem('authToken', token)
      localStorage.setItem('authUser', JSON.stringify(user))
      
      setToken(token)
      setUser(user)
    } finally {
      setIsLoading(false)
    }
  }

  const logout = async () => {
    try {
      await authApi.logout()
    } catch (error) {
      console.error('Logout error:', error)
    } finally {
      // Always clear local state regardless of API call result
      localStorage.removeItem('authToken')
      localStorage.removeItem('authUser')
      setToken(null)
      setUser(null)
    }
  }

  const isAuthenticated = !!user && !!token

  const value: AuthContextType = {
    user,
    token,
    isLoading,
    login,
    register,
    logout,
    isAuthenticated
  }

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  )
}

