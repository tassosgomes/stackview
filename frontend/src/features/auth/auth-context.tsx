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

    const init = async () => {
      if (storedToken && storedUser && storedUser !== 'undefined') {
        setToken(storedToken)
        try {
          setUser(JSON.parse(storedUser))
        } catch (error) {
          console.error('Error parsing stored user:', error)
          localStorage.removeItem('authToken')
          localStorage.removeItem('authUser')
        }
      } else if (storedToken && (!storedUser || storedUser === 'undefined')) {
        // Try to refresh user data from API when there is a token but no stored user
        try {
          const refreshed = await authApi.refreshToken()
          if (refreshed?.token) {
            localStorage.setItem('authToken', refreshed.token)
            setToken(refreshed.token)
          }
          if (refreshed?.user) {
            localStorage.setItem('authUser', JSON.stringify(refreshed.user))
            setUser(refreshed.user)
          }
        } catch (error) {
          console.error('Failed to refresh auth on init:', error)
          // If refresh fails, clear token to avoid inconsistent state
          localStorage.removeItem('authToken')
          localStorage.removeItem('authUser')
        }
      }
    }

    void init()

    setIsLoading(false)
  }, [])

  const login = async (credentials: LoginRequest) => {
    setIsLoading(true)
    try {
      let { user, token } = await authApi.login(credentials)

      // If the API returned no user (only token), try to refresh/populate user
      if (!user) {
        try {
          const refreshed = await authApi.refreshToken()
          if (refreshed?.token) token = refreshed.token
          if (refreshed?.user) user = refreshed.user
        } catch (err) {
          console.warn('No user returned on login and refreshToken failed', err)
        }
      }

      // Store token and user data (guard against undefined)
      if (token) {
        localStorage.setItem('authToken', token)
        setToken(token)
      }

      if (user) {
        localStorage.setItem('authUser', JSON.stringify(user))
        setUser(user)
      } else {
        localStorage.removeItem('authUser')
      }
    } finally {
      setIsLoading(false)
    }
  }

  const register = async (userData: RegisterRequest) => {
    setIsLoading(true)
    try {
      let { user, token } = await authApi.register(userData)

      // If register didn't return user (unlikely), attempt refresh
      if (!user) {
        try {
          const refreshed = await authApi.refreshToken()
          if (refreshed?.token) token = refreshed.token
          if (refreshed?.user) user = refreshed.user
        } catch (err) {
          console.warn('No user returned on register and refreshToken failed', err)
        }
      }

      // Store token and user data (guard against undefined)
      if (token) {
        localStorage.setItem('authToken', token)
        setToken(token)
      }

      if (user) {
        localStorage.setItem('authUser', JSON.stringify(user))
        setUser(user)
      } else {
        localStorage.removeItem('authUser')
      }
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

