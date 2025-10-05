import { Link } from "react-router-dom"
import { Card, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { useAuth } from "@/features/auth/use-auth"

export function HomePage() {
  const { isAuthenticated } = useAuth()

  return (
    <div className="space-y-8">
      <div className="text-center space-y-4">
        <h1 className="text-4xl font-bold tracking-tight">Welcome to StackShare</h1>
        <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
          Share and discover tech stacks from developers around the world. 
          Document your technology decisions and connect with AI assistants for smart queries.
        </p>
        
        {!isAuthenticated && (
          <div className="flex justify-center space-x-4 pt-4">
            <Button asChild size="lg">
              <Link to="/register">Get Started</Link>
            </Button>
            <Button asChild variant="outline" size="lg">
              <Link to="/login">Sign In</Link>
            </Button>
          </div>
        )}

        {isAuthenticated && (
          <div className="flex justify-center space-x-4 pt-4">
            <Button asChild size="lg">
              <Link to="/dashboard">Go to Dashboard</Link>
            </Button>
            <Button asChild variant="outline" size="lg">
              <Link to="/explore">Explore Stacks</Link>
            </Button>
          </div>
        )}
      </div>
      
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>Create Stacks</CardTitle>
            <CardDescription>
              Document your technology decisions with detailed descriptions in Markdown. 
              Organize by Frontend, Backend, Mobile, DevOps, Data, and Testing categories.
            </CardDescription>
          </CardHeader>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>Discover Technologies</CardTitle>
            <CardDescription>
              Find out how others are using your favorite technologies. 
              Search and filter by technology type to discover new approaches.
            </CardDescription>
          </CardHeader>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>AI Integration</CardTitle>
            <CardDescription>
              Generate personal access tokens to connect with AI assistants like 
              GitHub Copilot and Claude via Model Context Protocol (MCP).
            </CardDescription>
          </CardHeader>
        </Card>
      </div>
    </div>
  )
}