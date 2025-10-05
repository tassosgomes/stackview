import { Outlet } from 'react-router-dom'
import { cn } from '@/lib/utils'

interface LayoutProps {
  className?: string
}

export function Layout({ className }: LayoutProps) {
  return (
    <div className={cn("min-h-screen bg-background", className)}>
      <header className="border-b">
        <div className="container mx-auto px-4 py-4">
          <nav className="flex items-center justify-between">
            <div className="flex items-center space-x-4">
              <h1 className="text-2xl font-bold">StackShare</h1>
            </div>
            <div className="flex items-center space-x-4">
              {/* Navigation items will be added later */}
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