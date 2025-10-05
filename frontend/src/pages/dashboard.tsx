import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"

export function DashboardPage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Dashboard</h1>
        <p className="text-muted-foreground">
          Manage your stacks and MCP tokens
        </p>
      </div>
      
      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>My Stacks</CardTitle>
            <CardDescription>
              You haven't created any stacks yet
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button>Create Stack</Button>
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
            <Button>Generate Token</Button>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}