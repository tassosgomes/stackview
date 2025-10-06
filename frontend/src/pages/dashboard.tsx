import { Link } from "react-router-dom"
import { Plus, Eye, Edit } from "lucide-react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { useAuth } from "@/features/auth/use-auth"
import { useMyStacks } from "@/hooks/use-stacks"

export function DashboardPage() {
  const { user, logout } = useAuth()
  
  const {
    data: stacksResponse,
    isLoading: isLoadingStacks,
    error: stacksError
  } = useMyStacks(1, 10)

  const stacks = stacksResponse?.items ?? []

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-start">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">
            Bem-vindo, {user?.name}!
          </h1>
          <p className="text-muted-foreground">
            Gerencie seus stacks e tokens MCP
          </p>
        </div>
        <Button variant="outline" onClick={logout}>
          Sair
        </Button>
      </div>
      
      <div className="grid gap-6 md:grid-cols-2">
        <Card>
          <CardHeader>
            <CardTitle>Meus Stacks</CardTitle>
            <CardDescription>
              {stacks.length === 0 
                ? "Você ainda não criou nenhum stack" 
                : `Você tem ${stacks.length} stack${stacks.length === 1 ? '' : 's'}`
              }
            </CardDescription>
          </CardHeader>
          <CardContent>
            {isLoadingStacks ? (
              <div className="flex items-center space-x-2">
                <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-primary"></div>
                <span className="text-sm text-muted-foreground">Carregando stacks...</span>
              </div>
            ) : stacksError ? (
              <div className="text-sm text-red-600 bg-red-50 p-3 rounded">
                {stacksError instanceof Error ? stacksError.message : "Falha ao carregar seus stacks"}
              </div>
            ) : (
              <div className="space-y-4">
                {stacks.length === 0 ? (
                  <div className="text-center py-6">
                    <p className="text-sm text-muted-foreground mb-4">
                      Crie seu primeiro stack para começar
                    </p>
                    <Button asChild>
                      <Link to="/stacks/create">
                        <Plus className="h-4 w-4 mr-2" />
                        Criar Primeiro Stack
                      </Link>
                    </Button>
                  </div>
                ) : (
                  <div className="space-y-3">
                    {stacks.slice(0, 5).map((stack) => (
                      <div key={stack.id} className="flex items-center justify-between p-3 border rounded-md hover:bg-muted/50 transition-colors">
                        <div className="flex-1">
                          <div className="flex items-center space-x-2 mb-1">
                            <Link 
                              to={`/stacks/${stack.id}`}
                              className="font-medium text-sm hover:underline"
                            >
                              {stack.name}
                            </Link>
                            <Badge variant="secondary" className="text-xs">
                              {stack.type}
                            </Badge>
                            {!stack.isPublic && (
                              <Badge variant="outline" className="text-xs">
                                Privado
                              </Badge>
                            )}
                          </div>
                          <p className="text-xs text-muted-foreground">
                            {stack.technologyCount} tecnologia{stack.technologyCount === 1 ? '' : 's'} • 
                            Criado em {new Date(stack.createdAt).toLocaleDateString('pt-BR')}
                          </p>
                        </div>
                        <div className="flex gap-1">
                          <Button size="sm" variant="ghost" asChild>
                            <Link to={`/stacks/${stack.id}`}>
                              <Eye className="h-3 w-3" />
                            </Link>
                          </Button>
                          <Button size="sm" variant="ghost" asChild>
                            <Link to={`/stacks/${stack.id}/edit`}>
                              <Edit className="h-3 w-3" />
                            </Link>
                          </Button>
                        </div>
                      </div>
                    ))}
                    {stacks.length > 5 && (
                      <p className="text-xs text-muted-foreground text-center">
                        E mais {stacks.length - 5} stack{stacks.length - 5 === 1 ? '' : 's'}...
                      </p>
                    )}
                    <div className="flex gap-2">
                      <Button asChild className="flex-1">
                        <Link to="/stacks/create">
                          <Plus className="h-4 w-4 mr-2" />
                          Criar Stack
                        </Link>
                      </Button>
                      <Button variant="outline" asChild className="flex-1">
                        <Link to="/explore">
                          Explorar Stacks
                        </Link>
                      </Button>
                    </div>
                  </div>
                )}
              </div>
            )}
          </CardContent>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>Tokens MCP</CardTitle>
            <CardDescription>
              Gere tokens para integração com assistentes de IA
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Button disabled>Gerar Token</Button>
            <p className="text-xs text-muted-foreground mt-2">
              Disponível na Tarefa 12.0
            </p>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}