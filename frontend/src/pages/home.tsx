import { Link } from "react-router-dom"
import { Card, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { useAuth } from "@/features/auth/use-auth"

export function HomePage() {
  const { isAuthenticated } = useAuth()

  return (
    <div className="space-y-8">
      <div className="text-center space-y-4">
        <h1 className="text-4xl font-bold tracking-tight">Bem-vindo ao StackShare</h1>
        <p className="text-xl text-muted-foreground max-w-2xl mx-auto">
          Compartilhe e descubra tech stacks de desenvolvedores ao redor do mundo. 
          Documente suas decisões tecnológicas e conecte com assistentes de IA para consultas inteligentes.
        </p>
        
        {!isAuthenticated && (
          <div className="flex flex-col sm:flex-row justify-center gap-4 pt-4">
            <Button asChild size="lg">
              <Link to="/register">Começar</Link>
            </Button>
            <Button asChild variant="outline" size="lg">
              <Link to="/login">Entrar</Link>
            </Button>
            <Button asChild variant="secondary" size="lg">
              <Link to="/explore">Explorar Stacks</Link>
            </Button>
          </div>
        )}

        {isAuthenticated && (
          <div className="flex flex-col sm:flex-row justify-center gap-4 pt-4">
            <Button asChild size="lg">
              <Link to="/dashboard">Ir ao Dashboard</Link>
            </Button>
            <Button asChild variant="outline" size="lg">
              <Link to="/stacks/create">Criar Stack</Link>
            </Button>
            <Button asChild variant="secondary" size="lg">
              <Link to="/explore">Explorar Stacks</Link>
            </Button>
          </div>
        )}
      </div>
      
      <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>Criar Stacks</CardTitle>
            <CardDescription>
              Documente suas decisões tecnológicas com descrições detalhadas em Markdown. 
              Organize por categorias: Frontend, Backend, Mobile, DevOps, Data e Testing.
            </CardDescription>
          </CardHeader>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>Descobrir Tecnologias</CardTitle>
            <CardDescription>
              Descubra como outros estão usando suas tecnologias favoritas. 
              Busque e filtre por tipo de tecnologia para descobrir novas abordagens.
            </CardDescription>
          </CardHeader>
        </Card>
        
        <Card>
          <CardHeader>
            <CardTitle>Integração com IA</CardTitle>
            <CardDescription>
              Gere tokens de acesso pessoal para conectar com assistentes de IA como 
              GitHub Copilot e Claude via Model Context Protocol (MCP).
            </CardDescription>
          </CardHeader>
        </Card>
      </div>
    </div>
  )
}