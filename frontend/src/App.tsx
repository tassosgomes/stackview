import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'
import { ReactQueryProvider } from '@/lib/react-query'
import { Layout } from '@/components/layout'
import { HomePage } from '@/pages/home'
import { LoginPage } from '@/pages/login'
import { DashboardPage } from '@/pages/dashboard'

function App() {
  return (
    <ReactQueryProvider>
      <Router>
        <Routes>
          <Route path="/" element={<Layout />}>
            <Route index element={<HomePage />} />
            <Route path="login" element={<LoginPage />} />
            <Route path="dashboard" element={<DashboardPage />} />
            <Route path="*" element={<Navigate to="/" replace />} />
          </Route>
        </Routes>
      </Router>
    </ReactQueryProvider>
  )
}

export default App
