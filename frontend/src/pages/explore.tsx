import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Search, Filter, Plus } from 'lucide-react'
import { usePublicStacks } from '@/hooks/use-stacks'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import type { StackType } from '@/types/stack'

const stackTypes: StackType[] = [
  'Frontend',
  'Backend', 
  'Mobile',
  'DevOps',
  'Data',
  'Testing'
]

export default function ExplorePage() {
  const [page, setPage] = useState(1)
  const [search, setSearch] = useState('')
  const [typeFilter, setTypeFilter] = useState<string>('')
  const [technologyFilter, setTechnologyFilter] = useState('')

  const { data: stacksData, isLoading, isError } = usePublicStacks(
    page,
    12, // pageSize
    typeFilter || undefined,
    technologyFilter || undefined
  )

  const stacks = stacksData?.items || []
  const totalPages = stacksData?.totalPages || 0

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    setTechnologyFilter(search)
    setPage(1)
  }

  const clearFilters = () => {
    setSearch('')
    setTypeFilter('')
    setTechnologyFilter('')
    setPage(1)
  }

  if (isError) {
    return (
      <div className="container mx-auto py-6">
        <div className="text-center">
          <h2 className="text-2xl font-bold text-destructive">Erro ao carregar stacks</h2>
          <p className="text-muted-foreground mt-2">Tente novamente mais tarde</p>
        </div>
      </div>
    )
  }

  return (
    <div className="container mx-auto py-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
        <div>
          <h1 className="text-3xl font-bold">Explorar Stacks</h1>
          <p className="text-muted-foreground mt-1">
            Descubra tecnologias e arquiteturas de outros desenvolvedores
          </p>
        </div>
        <Button asChild>
          <Link to="/stacks/create">
            <Plus className="h-4 w-4 mr-2" />
            Criar Stack
          </Link>
        </Button>
      </div>

      {/* Filters */}
      <Card className="mb-6">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Filter className="h-5 w-5" />
            Filtros
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col lg:flex-row gap-4">
            {/* Search */}
            <form onSubmit={handleSearchSubmit} className="flex-1">
              <div className="flex gap-2">
                <Input
                  placeholder="Buscar por tecnologia ou nome..."
                  value={search}
                  onChange={(e) => setSearch(e.target.value)}
                  className="flex-1"
                />
                <Button type="submit" variant="outline">
                  <Search className="h-4 w-4" />
                </Button>
              </div>
            </form>

            {/* Type Filter */}
            <Select value={typeFilter} onValueChange={setTypeFilter}>
              <SelectTrigger className="w-full lg:w-48">
                <SelectValue placeholder="Tipo de Stack" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">Todos os tipos</SelectItem>
                {stackTypes.map((type) => (
                  <SelectItem key={type} value={type}>
                    {type}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            {/* Clear filters */}
            <Button variant="outline" onClick={clearFilters}>
              Limpar
            </Button>
          </div>

          {/* Active filters */}
          {(typeFilter || technologyFilter) && (
            <div className="flex gap-2 mt-4">
              <span className="text-sm text-muted-foreground">Filtros ativos:</span>
              {typeFilter && (
                <Badge variant="secondary" className="text-xs">
                  Tipo: {typeFilter}
                </Badge>
              )}
              {technologyFilter && (
                <Badge variant="secondary" className="text-xs">
                  Busca: {technologyFilter}
                </Badge>
              )}
            </div>
          )}
        </CardContent>
      </Card>

      {/* Loading State */}
      {isLoading && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {Array.from({ length: 6 }).map((_, i) => (
            <Card key={i} className="animate-pulse">
              <CardHeader>
                <div className="h-4 bg-muted rounded w-3/4"></div>
                <div className="h-3 bg-muted rounded w-1/2"></div>
              </CardHeader>
              <CardContent>
                <div className="h-3 bg-muted rounded w-full mb-2"></div>
                <div className="h-3 bg-muted rounded w-2/3"></div>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      {/* Stacks Grid */}
      {!isLoading && (
        <>
          {stacks.length === 0 ? (
            <Card>
              <CardContent className="text-center py-12">
                <h3 className="text-xl font-semibold mb-2">Nenhum stack encontrado</h3>
                <p className="text-muted-foreground mb-4">
                  {technologyFilter || typeFilter 
                    ? 'Tente ajustar os filtros para encontrar mais resultados.'
                    : 'Seja o primeiro a compartilhar um stack!'}
                </p>
                <Button asChild>
                  <Link to="/stacks/create">
                    <Plus className="h-4 w-4 mr-2" />
                    Criar Primeiro Stack
                  </Link>
                </Button>
              </CardContent>
            </Card>
          ) : (
            <>
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
                {stacks.map((stack) => (
                  <Card key={stack.id} className="hover:shadow-lg transition-shadow">
                    <CardHeader>
                      <div className="flex justify-between items-start">
                        <CardTitle className="text-lg">
                          <Link 
                            to={`/stacks/${stack.id}`}
                            className="hover:underline"
                          >
                            {stack.name}
                          </Link>
                        </CardTitle>
                        <Badge variant="outline" className="text-xs">
                          {stack.type}
                        </Badge>
                      </div>
                      <CardDescription>
                        {stack.technologyCount} tecnologia{stack.technologyCount !== 1 ? 's' : ''}
                      </CardDescription>
                    </CardHeader>
                    <CardContent>
                      <p className="text-sm text-muted-foreground">
                        Criado em {new Date(stack.createdAt).toLocaleDateString('pt-BR')}
                      </p>
                    </CardContent>
                  </Card>
                ))}
              </div>

              {/* Pagination */}
              {totalPages > 1 && (
                <div className="flex justify-center gap-2">
                  <Button
                    variant="outline"
                    disabled={page === 1}
                    onClick={() => setPage(page - 1)}
                  >
                    Anterior
                  </Button>
                  <span className="flex items-center px-4 text-sm">
                    Página {page} de {totalPages}
                  </span>
                  <Button
                    variant="outline"
                    disabled={page === totalPages}
                    onClick={() => setPage(page + 1)}
                  >
                    Próxima
                  </Button>
                </div>
              )}
            </>
          )}
        </>
      )}
    </div>
  )
}