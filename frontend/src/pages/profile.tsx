import { useAuth } from "@/features/auth/use-auth"
import { McpTokensSection } from "@/features/mcp-tokens"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { User, Mail } from "lucide-react"

export function ProfilePage() {
  const { user, logout } = useAuth()

  if (!user) {
    return null // This should be protected by the route guard
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-start">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Perfil</h1>
          <p className="text-muted-foreground">
            Gerencie suas informações e tokens de acesso
          </p>
        </div>
        <Button variant="outline" onClick={logout}>
          Sair
        </Button>
      </div>
      
      <div className="grid gap-6 lg:grid-cols-3">
        <div className="lg:col-span-1">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <User className="h-5 w-5" />
                Informações Pessoais
              </CardTitle>
              <CardDescription>
                Seus dados básicos de cadastro
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <label className="text-sm font-medium">Nome</label>
                <p className="text-sm text-muted-foreground">{user.name}</p>
              </div>
              <div className="space-y-2">
                <label className="text-sm font-medium flex items-center gap-1">
                  <Mail className="h-3 w-3" />
                  Email
                </label>
                <p className="text-sm text-muted-foreground">{user.email}</p>
              </div>
            </CardContent>
          </Card>
        </div>
        
        <div className="lg:col-span-2">
          <McpTokensSection />
        </div>
      </div>
    </div>
  )
}