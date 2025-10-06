import { useState, useMemo } from 'react'
import { X, Plus } from 'lucide-react'
import { Badge } from '@/components/ui/badge'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { useTechnologies, useSuggestTechnologies } from '@/hooks/use-stacks'
import type { Technology } from '@/types/stack'

interface TechnologySelectorProps {
  selectedTechnologies: Technology[]
  onTechnologiesChange: (technologies: Technology[]) => void
  className?: string
}

export function TechnologySelector({ 
  selectedTechnologies, 
  onTechnologiesChange,
  className 
}: TechnologySelectorProps) {
  const [inputValue, setInputValue] = useState('')
  const [showSuggestions, setShowSuggestions] = useState(false)

  const { data: allTechnologies = [] } = useTechnologies()
  const { data: suggestions = [] } = useSuggestTechnologies(inputValue)

  const filteredSuggestions = useMemo(() => {
    if (!inputValue) return allTechnologies
    
    // Combine suggestions from API with local filtering of all technologies
    const suggestedIds = suggestions.map(tech => tech.id)
    const localFiltered = allTechnologies.filter(tech => 
      tech.name.toLowerCase().includes(inputValue.toLowerCase()) &&
      !suggestedIds.includes(tech.id)
    )
    
    return [...suggestions, ...localFiltered]
      .filter(tech => !selectedTechnologies.some(selected => selected.id === tech.id))
      .slice(0, 10) // Limit suggestions
  }, [inputValue, suggestions, allTechnologies, selectedTechnologies])

  const handleAddTechnology = (technology: Technology) => {
    onTechnologiesChange([...selectedTechnologies, technology])
    setInputValue('')
    setShowSuggestions(false)
  }

  const handleRemoveTechnology = (technologyId: string) => {
    onTechnologiesChange(
      selectedTechnologies.filter(tech => tech.id !== technologyId)
    )
  }

  const handleCreateNewTechnology = () => {
    if (!inputValue.trim()) return
    
    // Create a temporary technology object
    // In a real app, this would call an API to create the technology
    const newTech: Technology = {
      id: `temp-${Date.now()}`, // Temporary ID
      name: inputValue.trim(),
      category: 'Library' // Default category
    }
    
    handleAddTechnology(newTech)
  }

  return (
    <div className={className}>
      <Label htmlFor="technologies">Tecnologias</Label>
      
      {/* Selected Technologies */}
      {selectedTechnologies.length > 0 && (
        <div className="flex flex-wrap gap-2 mb-2">
          {selectedTechnologies.map((tech) => (
            <Badge key={tech.id} variant="secondary" className="flex items-center gap-1">
              {tech.name}
              <button
                type="button"
                onClick={() => handleRemoveTechnology(tech.id)}
                className="ml-1 hover:text-destructive"
              >
                <X className="h-3 w-3" />
              </button>
            </Badge>
          ))}
        </div>
      )}

      {/* Input with suggestions */}
      <div className="relative">
        <Input
          id="technologies"
          type="text"
          placeholder="Digite o nome da tecnologia..."
          value={inputValue}
          onChange={(e) => {
            setInputValue(e.target.value)
            setShowSuggestions(true)
          }}
          onFocus={() => setShowSuggestions(true)}
          onBlur={() => {
            // Delay hiding suggestions to allow clicking
            setTimeout(() => setShowSuggestions(false), 200)
          }}
        />

        {/* Suggestions dropdown */}
        {showSuggestions && (inputValue.length >= 2 || filteredSuggestions.length > 0) && (
          <div className="absolute top-full left-0 right-0 z-50 bg-popover border rounded-md shadow-lg mt-1 max-h-60 overflow-y-auto">
            {filteredSuggestions.length > 0 ? (
              filteredSuggestions.map((tech) => (
                <button
                  key={tech.id}
                  type="button"
                  className="w-full text-left px-3 py-2 hover:bg-accent hover:text-accent-foreground text-sm"
                  onClick={() => handleAddTechnology(tech)}
                >
                  <div>
                    <div className="font-medium">{tech.name}</div>
                    <div className="text-xs text-muted-foreground">{tech.category}</div>
                  </div>
                </button>
              ))
            ) : inputValue.length >= 2 ? (
              <div className="px-3 py-2">
                <div className="text-sm text-muted-foreground mb-2">
                  Nenhuma tecnologia encontrada para "{inputValue}"
                </div>
                <Button
                  type="button"
                  variant="outline"
                  size="sm"
                  className="w-full"
                  onClick={handleCreateNewTechnology}
                >
                  <Plus className="h-4 w-4 mr-1" />
                  Adicionar "{inputValue}"
                </Button>
              </div>
            ) : null}
          </div>
        )}
      </div>
    </div>
  )
}