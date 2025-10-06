import { useParams, useNavigate, Link } from 'react-router-dom'
import { ArrowLeft } from 'lucide-react'
import { toast } from 'sonner'
import { useStack, useUpdateStack } from '@/hooks/use-stacks'
import { StackForm } from '@/components/stack-form'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { useAuth } from '@/features/auth/use-auth'
import type { UpdateStackRequest } from '@/types/stack'

export default function EditStackPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { user } = useAuth()
  
  const { data: stack, isLoading, isError } = useStack(id!)
  const updateStackMutation = useUpdateStack()

  const handleSubmit = async (formData: any) => {
    if (!stack) return
    
    try {
      const stackData: UpdateStackRequest = {
        name: formData.name,
        description: formData.description,
        type: formData.type,
        isPublic: formData.isPublic,
        technologyIds: formData.technologies.map((tech: any) => tech.id)
      }

      await updateStackMutation.mutateAsync({ id: stack.id, data: stackData })
      toast.success('Stack atualizado com sucesso!')
      navigate(`/stacks/${stack.id}`)
    } catch (error) {
      toast.error('Erro ao atualizar stack. Tente novamente.')
      console.error('Error updating stack:', error)
    }
  }

  if (isLoading) {
    return (
      <div className="container mx-auto py-6 max-w-4xl">
        <div className="animate-pulse">
          <div className="h-8 bg-muted rounded w-1/3 mb-4"></div>
          <Card>
            <CardHeader>
              <div className="h-6 bg-muted rounded w-1/2 mb-2"></div>
              <div className="h-4 bg-muted rounded w-1/3"></div>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div className="h-10 bg-muted rounded"></div>
                <div className="h-10 bg-muted rounded"></div>
                <div className="h-32 bg-muted rounded"></div>
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
    )
  }

  if (isError || !stack) {
    return (
      <div className="container mx-auto py-6 max-w-4xl">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-destructive">Stack não encontrado</h2>
          <p className="text-muted-foreground mt-2 mb-4">
            Este stack pode ter sido removido ou você não tem permissão para editá-lo.
          </p>
          <Button asChild variant="outline">
            <Link to="/dashboard">
              <ArrowLeft className="h-4 w-4 mr-2" />
              Voltar ao dashboard
            </Link>
          </Button>
        </div>
      </div>
    )
  }

  // Check if user is the owner
  if (user?.id !== stack.userId) {
    return (
      <div className="container mx-auto py-6 max-w-4xl">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-destructive">Acesso negado</h2>
          <p className="text-muted-foreground mt-2 mb-4">
            Você não tem permissão para editar este stack.
          </p>
          <Button asChild variant="outline">
            <Link to={`/stacks/${stack.id}`}>
              <ArrowLeft className="h-4 w-4 mr-2" />
              Voltar ao stack
            </Link>
          </Button>
        </div>
      </div>
    )
  }

  return (
    <div className="container mx-auto py-6 max-w-4xl">
      {/* Header */}
      <div className="flex items-center gap-4 mb-6">
        <Button variant="ghost" size="sm" asChild>
          <Link to={`/stacks/${stack.id}`}>
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar ao stack
          </Link>
        </Button>
      </div>

      <div className="mb-6">
        <h1 className="text-3xl font-bold">Editar Stack</h1>
        <p className="text-muted-foreground mt-2">
          Atualize as informações do seu stack "{stack.name}"
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Detalhes do Stack</CardTitle>
          <CardDescription>
            Modifique as informações sobre as tecnologias e descrição do seu stack
          </CardDescription>
        </CardHeader>
        <CardContent>
          <StackForm 
            stack={stack}
            onSubmit={handleSubmit}
            isLoading={updateStackMutation.isPending}
            submitLabel="Atualizar Stack"
          />
        </CardContent>
      </Card>
    </div>
  )
}