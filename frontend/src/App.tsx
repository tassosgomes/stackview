import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'
import { ReactQueryProvider } from '@/lib/react-query'
import { AuthProvider } from '@/features/auth/auth-context'
import { ProtectedRoute } from '@/features/auth/protected-route'
import { Layout } from '@/components/layout'
import { HomePage } from '@/pages/home'
import { LoginPage } from '@/pages/login'
import { RegisterPage } from '@/pages/register'
import { DashboardPage } from '@/pages/dashboard'
import ExplorePage from '@/pages/explore'
import CreateStackPage from '@/pages/create-stack'
import StackDetailPage from '@/pages/stack-detail'
import EditStackPage from '@/pages/edit-stack'
import { Toaster } from 'sonner'

function App() {
  return (
    <ReactQueryProvider>
      <AuthProvider>
        <Router>
          <Routes>
            <Route path="/" element={<Layout />}>
              <Route index element={<HomePage />} />
              <Route path="login" element={<LoginPage />} />
              <Route path="register" element={<RegisterPage />} />
              <Route path="explore" element={<ExplorePage />} />
              <Route path="stacks/:id" element={<StackDetailPage />} />
              <Route path="dashboard" element={
                <ProtectedRoute>
                  <DashboardPage />
                </ProtectedRoute>
              } />
              <Route path="stacks/create" element={
                <ProtectedRoute>
                  <CreateStackPage />
                </ProtectedRoute>
              } />
              <Route path="stacks/:id/edit" element={
                <ProtectedRoute>
                  <EditStackPage />
                </ProtectedRoute>
              } />
              <Route path="*" element={<Navigate to="/" replace />} />
            </Route>
          </Routes>
          <Toaster />
        </Router>
      </AuthProvider>
    </ReactQueryProvider>
  )
}

export default App
