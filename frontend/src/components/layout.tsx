import { Link, Outlet, useLocation } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { useAuth } from '@/features/auth/use-auth'
import { cn } from '@/lib/utils'

interface LayoutProps {
  className?: string
}

export function Layout({ className }: LayoutProps) {
  const { isAuthenticated, user, logout } = useAuth()
  const location = useLocation()

  return (
    <div className={cn("min-h-screen bg-background", className)}>
      <header className="border-b">
        <div className="container mx-auto px-4 py-4">
          <nav className="flex items-center justify-between">
            <div className="flex items-center space-x-6">
              <Link to="/" className="text-2xl font-bold hover:text-primary transition-colors">
                StackShare
              </Link>
              
              <div className="hidden md:flex items-center space-x-4">
                <Button 
                  asChild 
                  variant={location.pathname === '/explore' ? 'default' : 'ghost'}
                  size="sm"
                >
                  <Link to="/explore">Explorar</Link>
                </Button>
                {isAuthenticated && (
                  <Button 
                    asChild 
                    variant={location.pathname === '/dashboard' ? 'default' : 'ghost'}
                    size="sm"
                  >
                    <Link to="/dashboard">Dashboard</Link>
                  </Button>
                )}
              </div>
            </div>
            
            <div className="flex items-center space-x-4">
              {isAuthenticated ? (
                <>
                  <span className="text-sm text-muted-foreground hidden sm:inline">
                    Olá, {user?.name}
                  </span>
                  <Button asChild variant="ghost">
                    <Link to="/dashboard">Dashboard</Link>
                  </Button>
                  <Button asChild variant="ghost">
                    <Link to="/profile">Perfil</Link>
                  </Button>
                  <Button variant="outline" onClick={logout}>
                    Sair
                  </Button>
                </>
              ) : (
                <>
                  <Button asChild variant="ghost">
                    <Link to="/login">Entrar</Link>
                  </Button>
                  <Button asChild>
                    <Link to="/register">Cadastrar</Link>
                  </Button>
                </>
              )}
            </div>
          </nav>
        </div>
      </header>
      
      <main className="container mx-auto px-4 py-8">
        <Outlet />
      </main>
    </div>
  )
}