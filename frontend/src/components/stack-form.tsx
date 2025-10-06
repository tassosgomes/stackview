import { useForm } from 'react-hook-form'
import { zodResolver } from '@hookform/resolvers/zod'
import { z } from 'zod'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Switch } from '@/components/ui/switch'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from '@/components/ui/form'
import { MarkdownEditor } from '@/components/markdown-editor'
import { TechnologySelector } from '@/components/technology-selector'
import type { Stack, StackType } from '@/types/stack'

const stackTypes: StackType[] = [
  'Frontend',
  'Backend', 
  'Mobile',
  'DevOps',
  'Data',
  'Testing'
]

const stackFormSchema = z.object({
  name: z.string().min(1, 'Nome é obrigatório').max(100, 'Nome muito longo'),
  description: z.string().min(1, 'Descrição é obrigatória'),
  type: z.enum(['Frontend', 'Backend', 'Mobile', 'DevOps', 'Data', 'Testing']),
  isPublic: z.boolean(),
  technologies: z.array(z.object({
    id: z.string(),
    name: z.string(),
    category: z.enum(['Language', 'Framework', 'Library', 'Tool', 'Database', 'Cloud'])
  })).min(1, 'Selecione pelo menos uma tecnologia')
})

type StackFormData = z.infer<typeof stackFormSchema>

interface StackFormProps {
  stack?: Stack
  onSubmit: (data: StackFormData) => void
  isLoading?: boolean
  submitLabel?: string
}

export function StackForm({ 
  stack, 
  onSubmit, 
  isLoading = false,
  submitLabel = stack ? 'Atualizar Stack' : 'Criar Stack'
}: StackFormProps) {
  const form = useForm<StackFormData>({
    resolver: zodResolver(stackFormSchema),
    defaultValues: {
      name: stack?.name || '',
      description: stack?.description || '',
      type: stack?.type || 'Frontend',
      isPublic: stack?.isPublic || false,
      technologies: stack?.technologies || []
    }
  })

  const handleSubmit = (data: StackFormData) => {
    onSubmit(data)
  }

  return (
    <Form {...form}>
      <form onSubmit={form.handleSubmit(handleSubmit)} className="space-y-6">
        
        {/* Name Field */}
        <FormField
          control={form.control}
          name="name"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Nome do Stack</FormLabel>
              <FormControl>
                <Input 
                  placeholder="Ex: E-commerce Moderno" 
                  {...field} 
                />
              </FormControl>
              <FormDescription>
                Um nome descritivo para seu stack tecnológico
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Type Field */}
        <FormField
          control={form.control}
          name="type"
          render={({ field }) => (
            <FormItem>
              <FormLabel>Tipo do Stack</FormLabel>
              <Select onValueChange={field.onChange} defaultValue={field.value}>
                <FormControl>
                  <SelectTrigger>
                    <SelectValue placeholder="Selecione o tipo" />
                  </SelectTrigger>
                </FormControl>
                <SelectContent>
                  {stackTypes.map((type) => (
                    <SelectItem key={type} value={type}>
                      {type}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              <FormDescription>
                Categoria principal do seu stack
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Technologies Field */}
        <FormField
          control={form.control}
          name="technologies"
          render={({ field }) => (
            <FormItem>
              <FormControl>
                <TechnologySelector
                  selectedTechnologies={field.value}
                  onTechnologiesChange={field.onChange}
                />
              </FormControl>
              <FormDescription>
                Adicione as tecnologias utilizadas neste stack
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Description Field */}
        <FormField
          control={form.control}
          name="description"
          render={({ field }) => (
            <FormItem>
              <FormControl>
                <MarkdownEditor
                  value={field.value}
                  onChange={field.onChange}
                  placeholder="Descreva seu stack, arquitetura, decisões técnicas..."
                />
              </FormControl>
              <FormDescription>
                Use Markdown para formatar sua descrição. Inclua detalhes sobre arquitetura, padrões e decisões técnicas.
              </FormDescription>
              <FormMessage />
            </FormItem>
          )}
        />

        {/* Public Switch */}
        <FormField
          control={form.control}
          name="isPublic"
          render={({ field }) => (
            <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
              <div className="space-y-0.5">
                <FormLabel className="text-base">Stack Público</FormLabel>
                <FormDescription>
                  Torne este stack visível para outros usuários
                </FormDescription>
              </div>
              <FormControl>
                <Switch
                  checked={field.value}
                  onCheckedChange={field.onChange}
                />
              </FormControl>
            </FormItem>
          )}
        />

        {/* Submit Button */}
        <Button type="submit" className="w-full" disabled={isLoading}>
          {isLoading ? 'Salvando...' : submitLabel}
        </Button>
      </form>
    </Form>
  )
}