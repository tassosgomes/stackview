import { Card, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"

export function HomePage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-4xl font-bold tracking-tight">Welcome to StackShare</h1>
        <p className="text-xl text-muted-foreground mt-2">
          Share and discover tech stacks from developers around the world
        </p>
      </div>
      
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>Create Stacks</CardTitle>
            <CardDescription>
              Document your technology decisions and share them with the community
            </CardDescription>
          </CardHeader>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>Discover Technologies</CardTitle>
            <CardDescription>
              Find out how others are using your favorite technologies
            </CardDescription>
          </CardHeader>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>AI Integration</CardTitle>
            <CardDescription>
              Connect with AI assistants via MCP tokens for smart queries
            </CardDescription>
          </CardHeader>
        </Card>
      </div>
    </div>
  )
}