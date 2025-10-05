import { useState, useEffect } from "react"
import { Link } from "react-router-dom"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { useAuth } from "@/features/auth/use-auth"
import { stacksApi } from "@/services/stacks"
import type { StackSummary } from "@/types/stack"

export function DashboardPage() {
  const { user, logout } = useAuth()
  const [stacks, setStacks] = useState<StackSummary[]>([])
  const [isLoadingStacks, setIsLoadingStacks] = useState(true)
  const [stacksError, setStacksError] = useState<string>("")

  useEffect(() => {
    const loadStacks = async () => {
      try {
        setIsLoadingStacks(true)
        const response = await stacksApi.getMyStacks(1, 10)
        setStacks(response.items)
      } catch (error) {
        setStacksError(
          error instanceof Error 
            ? error.message 
            : "Failed to load your stacks"
        )
      } finally {
        setIsLoadingStacks(false)
      }
    }

    loadStacks()
  }, [])

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-start">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">
            Welcome back, {user?.name}!
          </h1>
          <p className="text-muted-foreground">
            Manage your stacks and MCP tokens
          </p>
        </div>
        <Button variant="outline" onClick={logout}>
          Sign Out
        </Button>
      </div>
      
      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>My Stacks</CardTitle>
            <CardDescription>
              {stacks.length === 0 
                ? "You haven't created any stacks yet" 
                : `You have ${stacks.length} stack${stacks.length === 1 ? '' : 's'}`
              }
            </CardDescription>
          </CardHeader>
          <CardContent>
            {isLoadingStacks ? (
              <div className="flex items-center space-x-2">
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-primary"></div>
                <span className="text-sm text-muted-foreground">Loading stacks...</span>
              </div>
            ) : stacksError ? (
              <div className="text-sm text-red-600 bg-red-50 p-3 rounded">
                {stacksError}
              </div>
            ) : (
              <div className="space-y-4">
                {stacks.length === 0 ? (
                  <p className="text-sm text-muted-foreground">
                    Create your first stack to get started
                  </p>
                ) : (
                  <div className="space-y-2">
                    {stacks.slice(0, 3).map((stack) => (
                      <div key={stack.id} className="flex items-center justify-between p-3 border rounded-md">
                        <div className="flex-1">
                          <div className="flex items-center space-x-2">
                            <span className="font-medium text-sm">{stack.name}</span>
                            <Badge variant="secondary" className="text-xs">
                              {stack.type}
                            </Badge>
                            {!stack.isPublic && (
                              <Badge variant="outline" className="text-xs">
                                Private
                              </Badge>
                            )}
                          </div>
                          <p className="text-xs text-muted-foreground">
                            {stack.technologyCount} technolog{stack.technologyCount === 1 ? 'y' : 'ies'}
                          </p>
                        </div>
                      </div>
                    ))}
                    {stacks.length > 3 && (
                      <p className="text-xs text-muted-foreground">
                        And {stacks.length - 3} more...
                      </p>
                    )}
                  </div>
                )}
                <Button asChild className="w-full">
                  <Link to="/stacks/new">Create Stack</Link>
                </Button>
                {stacks.length > 0 && (
                  <Button variant="outline" asChild className="w-full">
                    <Link to="/stacks">View All Stacks</Link>
                  </Button>
                )}
              </div>
            )}
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>MCP Tokens</CardTitle>
            <CardDescription>
              Generate tokens for AI assistant integration
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button disabled>Generate Token</Button>
            <p className="text-xs text-muted-foreground mt-2">
              Coming in Task 12.0
            </p>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}