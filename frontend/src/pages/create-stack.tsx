import { useNavigate } from 'react-router-dom'
import { toast } from 'sonner'
import { StackForm } from '@/components/stack-form'
import { useCreateStack } from '@/hooks/use-stacks'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import type { CreateStackRequest, Technology, StackType } from '@/types/stack'

interface StackFormData {
  name: string
  description: string
  type: StackType
  isPublic: boolean
  technologies: Technology[]
}

export default function CreateStackPage() {
  const navigate = useNavigate()
  const createStackMutation = useCreateStack()

  const handleSubmit = async (formData: StackFormData) => {
    try {
      const stackData: CreateStackRequest = {
        name: formData.name,
        description: formData.description,
        type: formData.type,
        isPublic: formData.isPublic,
        technologyIds: formData.technologies.map((tech: Technology) => tech.id)
      }

      await createStackMutation.mutateAsync(stackData)
      toast.success('Stack criado com sucesso!')
      navigate('/dashboard')
    } catch (error) {
      toast.error('Erro ao criar stack. Tente novamente.')
      console.error('Error creating stack:', error)
    }
  }

  return (
    <div className="container mx-auto py-6 max-w-4xl">
      <div className="mb-6">
        <h1 className="text-3xl font-bold">Criar Novo Stack</h1>
        <p className="text-muted-foreground mt-2">
          Documente as tecnologias e arquitetura do seu projeto
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Detalhes do Stack</CardTitle>
          <CardDescription>
            Preencha as informações sobre as tecnologias e descrição do seu stack
          </CardDescription>
        </CardHeader>
        <CardContent>
          <StackForm 
            onSubmit={handleSubmit}
            isLoading={createStackMutation.isPending}
            submitLabel="Criar Stack"
          />
        </CardContent>
      </Card>
    </div>
  )
}