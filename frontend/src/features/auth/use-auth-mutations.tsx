import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { authApi } from '@/services/auth'


export function useLogin() {
  const navigate = useNavigate()
  
  return useMutation({
    mutationFn: authApi.login,
    onSuccess: (data) => {
      // Store token and user data
      localStorage.setItem('authToken', data.token)
      localStorage.setItem('authUser', JSON.stringify(data.user))
      navigate('/dashboard')
    },
    onError: (error) => {
      console.error('Login failed:', error)
    }
  })
}

export function useRegister() {
  const navigate = useNavigate()
  
  return useMutation({
    mutationFn: authApi.register,
    onSuccess: (data) => {
      // Store token and user data
      localStorage.setItem('authToken', data.token)
      localStorage.setItem('authUser', JSON.stringify(data.user))
      navigate('/dashboard')
    },
    onError: (error) => {
      console.error('Registration failed:', error)
    }
  })
}

export function useLogout() {
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  
  return useMutation({
    mutationFn: authApi.logout,
    onSettled: () => {
      // Always clear local state regardless of API call result
      localStorage.removeItem('authToken')
      localStorage.removeItem('authUser')
      queryClient.clear()
      navigate('/')
    }
  })
}