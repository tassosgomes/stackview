import { useParams, Link, useNavigate } from 'react-router-dom'
import { Edit, Trash2, ArrowLeft } from 'lucide-react'
import ReactMarkdown from 'react-markdown'
import remarkGfm from 'remark-gfm'
import { useStack, useDeleteStack } from '@/hooks/use-stacks'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { toast } from 'sonner'
import { useAuth } from '@/features/auth/use-auth'

export default function StackDetailPage() {
  const { id } = useParams<{ id: string }>()
  const navigate = useNavigate()
  const { user } = useAuth()
  
  const { data: stack, isLoading, isError } = useStack(id!)
  const deleteStackMutation = useDeleteStack()

  const handleDelete = async () => {
    if (!stack) return
    
    if (window.confirm('Tem certeza que deseja excluir este stack? Esta ação não pode ser desfeita.')) {
      try {
        await deleteStackMutation.mutateAsync(stack.id)
        toast.success('Stack excluído com sucesso!')
        navigate('/dashboard')
      } catch (error) {
        toast.error('Erro ao excluir stack. Tente novamente.')
        console.error('Error deleting stack:', error)
      }
    }
  }

  if (isLoading) {
    return (
      <div className="container mx-auto py-6 max-w-4xl">
        <div className="animate-pulse">
          <div className="h-8 bg-muted rounded w-1/3 mb-4"></div>
          <div className="h-4 bg-muted rounded w-1/4 mb-8"></div>
          <Card>
            <CardHeader>
              <div className="h-6 bg-muted rounded w-1/2 mb-2"></div>
              <div className="h-4 bg-muted rounded w-1/3"></div>
            </CardHeader>
            <CardContent>
              <div className="space-y-2">
                <div className="h-4 bg-muted rounded w-full"></div>
                <div className="h-4 bg-muted rounded w-3/4"></div>
                <div className="h-4 bg-muted rounded w-1/2"></div>
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
            Este stack pode ter sido removido ou você não tem permissão para visualizá-lo.
          </p>
          <Button asChild variant="outline">
            <Link to="/explore">
              <ArrowLeft className="h-4 w-4 mr-2" />
              Voltar para explorar
            </Link>
          </Button>
        </div>
      </div>
    )
  }

  const isOwner = user?.id === stack.userId
  const canEdit = isOwner
  const canDelete = isOwner

  return (
    <div className="container mx-auto py-6 max-w-4xl">
      {/* Header */}
      <div className="flex items-center gap-4 mb-6">
        <Button variant="ghost" size="sm" asChild>
          <Link to={isOwner ? "/dashboard" : "/explore"}>
            <ArrowLeft className="h-4 w-4 mr-2" />
            Voltar
          </Link>
        </Button>
      </div>

      {/* Stack Details */}
      <Card className="mb-6">
        <CardHeader>
          <div className="flex justify-between items-start">
            <div>
              <CardTitle className="text-2xl">{stack.name}</CardTitle>
              <CardDescription className="mt-2">
                <div className="flex items-center gap-4 text-sm">
                  <Badge variant="outline">{stack.type}</Badge>
                  <span>
                    {stack.isPublic ? 'Público' : 'Privado'}
                  </span>
                  <span>
                    Criado em {new Date(stack.createdAt).toLocaleDateString('pt-BR')}
                  </span>
                  {stack.updatedAt !== stack.createdAt && (
                    <span>
                      Atualizado em {new Date(stack.updatedAt).toLocaleDateString('pt-BR')}
                    </span>
                  )}
                </div>
              </CardDescription>
            </div>

            {/* Action buttons for owner */}
            {(canEdit || canDelete) && (
              <div className="flex gap-2">
                {canEdit && (
                  <Button variant="outline" size="sm" asChild>
                    <Link to={`/stacks/${stack.id}/edit`}>
                      <Edit className="h-4 w-4 mr-2" />
                      Editar
                    </Link>
                  </Button>
                )}
                {canDelete && (
                  <Button 
                    variant="outline" 
                    size="sm"
                    onClick={handleDelete}
                    disabled={deleteStackMutation.isPending}
                  >
                    <Trash2 className="h-4 w-4 mr-2" />
                    Excluir
                  </Button>
                )}
              </div>
            )}
          </div>
        </CardHeader>
      </Card>

      {/* Technologies */}
      <Card className="mb-6">
        <CardHeader>
          <CardTitle>Tecnologias</CardTitle>
          <CardDescription>
            {stack.technologies.length} tecnologia{stack.technologies.length !== 1 ? 's' : ''} utilizada{stack.technologies.length !== 1 ? 's' : ''}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex flex-wrap gap-2">
            {stack.technologies.map((tech) => (
              <Badge key={tech.id} variant="secondary" className="text-sm">
                {tech.name}
                <span className="ml-1 text-xs text-muted-foreground">
                  ({tech.category})
                </span>
              </Badge>
            ))}
          </div>
        </CardContent>
      </Card>

      {/* Description */}
      <Card>
        <CardHeader>
          <CardTitle>Descrição</CardTitle>
        </CardHeader>
        <CardContent>
          {stack.description ? (
            <div className="prose prose-sm max-w-none dark:prose-invert">
              <ReactMarkdown remarkPlugins={[remarkGfm]}>
                {stack.description}
              </ReactMarkdown>
            </div>
          ) : (
            <p className="text-muted-foreground italic">
              Nenhuma descrição fornecida.
            </p>
          )}
        </CardContent>
      </Card>
    </div>
  )
}