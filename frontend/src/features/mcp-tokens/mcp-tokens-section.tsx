import { useState } from 'react'
import { Copy, Key, Trash2, Calendar, AlertTriangle } from 'lucide-react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog'
import { useMcpTokens, useCreateMcpToken, useRevokeMcpToken } from '@/hooks/use-mcp-tokens'
import { toast } from 'sonner'

export function McpTokensSection() {
  const { data: tokens = [], isLoading, error } = useMcpTokens()
  const createTokenMutation = useCreateMcpToken()
  const revokeTokenMutation = useRevokeMcpToken()

  const [showTokenDialog, setShowTokenDialog] = useState(false)
  const [newTokenData, setNewTokenData] = useState<string | null>(null)

  const handleCreateToken = async () => {
    try {
      const result = await createTokenMutation.mutateAsync()
      setNewTokenData(result.rawToken)
      setShowTokenDialog(true)
    } catch {
      // Error is handled by the mutation hook
    }
  }

  const handleCopyToken = async () => {
    if (newTokenData) {
      try {
        await navigator.clipboard.writeText(newTokenData)
        toast.success('Token copiado para a área de transferência')
      } catch {
        toast.error('Falha ao copiar token')
      }
    }
  }

  const handleCloseTokenDialog = () => {
    setShowTokenDialog(false)
    setNewTokenData(null)
  }

  const handleRevokeToken = async (tokenId: string) => {
    try {
      await revokeTokenMutation.mutateAsync(tokenId)
    } catch {
      // Error is handled by the mutation hook
    }
  }

  const activeTokens = tokens.filter(token => !token.isRevoked)
  const revokedTokens = tokens.filter(token => token.isRevoked)

  if (isLoading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Tokens MCP</CardTitle>
          <CardDescription>
            Gere tokens para integração com assistentes de IA
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="flex items-center space-x-2">
            <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-primary"></div>
            <span className="text-sm text-muted-foreground">Carregando tokens...</span>
          </div>
        </CardContent>
      </Card>
    )
  }

  if (error) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Tokens MCP</CardTitle>
          <CardDescription>
            Gere tokens para integração com assistentes de IA
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="text-sm text-red-600 bg-red-50 p-3 rounded">
            {error instanceof Error ? error.message : "Falha ao carregar tokens"}
          </div>
        </CardContent>
      </Card>
    )
  }

  return (
    <>
      <Card>
        <CardHeader>
          <CardTitle>Tokens MCP</CardTitle>
          <CardDescription>
            Gere tokens para integração com assistentes de IA como GitHub Copilot e Claude Desktop
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="space-y-4">
            <Button 
              onClick={handleCreateToken} 
              disabled={createTokenMutation.isPending}
              className="w-full"
            >
              <Key className="h-4 w-4 mr-2" />
              {createTokenMutation.isPending ? 'Gerando...' : 'Gerar Novo Token'}
            </Button>
            
            {activeTokens.length > 0 && (
              <div className="space-y-3">
                <h4 className="text-sm font-medium">Tokens Ativos ({activeTokens.length})</h4>
                {activeTokens.map((token) => (
                  <div key={token.id} className="flex items-center justify-between p-3 border rounded-md">
                    <div className="flex items-center space-x-3">
                      <Key className="h-4 w-4 text-muted-foreground" />
                      <div>
                        <div className="flex items-center space-x-2">
                          <span className="text-sm font-mono">
                            ****{token.id.slice(-4)}
                          </span>
                          <Badge variant="secondary">Ativo</Badge>
                        </div>
                        <div className="flex items-center space-x-1 text-xs text-muted-foreground">
                          <Calendar className="h-3 w-3" />
                          <span>Criado em {new Date(token.createdAt).toLocaleDateString('pt-BR')}</span>
                        </div>
                      </div>
                    </div>
                    <Dialog>
                      <DialogTrigger asChild>
                        <Button 
                          variant="destructive" 
                          size="sm"
                        >
                          <Trash2 className="h-3 w-3" />
                        </Button>
                      </DialogTrigger>
                      <DialogContent>
                        <DialogHeader>
                          <DialogTitle>Revogar Token</DialogTitle>
                          <DialogDescription>
                            Tem certeza que deseja revogar este token? Esta ação não pode ser desfeita e 
                            qualquer integração usando este token parará de funcionar.
                          </DialogDescription>
                        </DialogHeader>
                        <DialogFooter>
                          <DialogTrigger asChild>
                            <Button variant="outline">
                              Cancelar
                            </Button>
                          </DialogTrigger>
                          <Button 
                            variant="destructive" 
                            onClick={() => token.id && handleRevokeToken(token.id)}
                            disabled={revokeTokenMutation.isPending}
                          >
                            {revokeTokenMutation.isPending ? 'Revogando...' : 'Revogar Token'}
                          </Button>
                        </DialogFooter>
                      </DialogContent>
                    </Dialog>
                  </div>
                ))}
              </div>
            )}

            {revokedTokens.length > 0 && (
              <div className="space-y-3">
                <h4 className="text-sm font-medium text-muted-foreground">
                  Tokens Revogados ({revokedTokens.length})
                </h4>
                {revokedTokens.map((token) => (
                  <div key={token.id} className="flex items-center space-x-3 p-3 border rounded-md opacity-60">
                    <Key className="h-4 w-4 text-muted-foreground" />
                    <div>
                      <div className="flex items-center space-x-2">
                        <span className="text-sm font-mono">
                          ****{token.id.slice(-4)}
                        </span>
                        <Badge variant="outline">Revogado</Badge>
                      </div>
                      <div className="flex items-center space-x-1 text-xs text-muted-foreground">
                        <Calendar className="h-3 w-3" />
                        <span>Criado em {new Date(token.createdAt).toLocaleDateString('pt-BR')}</span>
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            )}

            {activeTokens.length === 0 && revokedTokens.length === 0 && (
              <div className="text-center py-6">
                <Key className="h-8 w-8 text-muted-foreground mx-auto mb-2" />
                <p className="text-sm text-muted-foreground">
                  Você ainda não tem nenhum token MCP
                </p>
              </div>
            )}
          </div>
        </CardContent>
      </Card>

      {/* Token Generated Dialog */}
      <Dialog open={showTokenDialog} onOpenChange={setShowTokenDialog}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Token Gerado com Sucesso</DialogTitle>
            <DialogDescription>
              Este é o seu novo token MCP. Copie-o agora, pois ele não será mostrado novamente.
            </DialogDescription>
          </DialogHeader>
          
          <div className="space-y-4">
            <div className="flex items-center space-x-2 p-3 bg-muted rounded-md">
              <AlertTriangle className="h-4 w-4 text-amber-500 flex-shrink-0" />
              <p className="text-xs text-muted-foreground">
                Guarde este token em local seguro. Ele não poderá ser visualizado novamente após fechar esta janela.
              </p>
            </div>
            
            <div className="space-y-2">
              <label className="text-sm font-medium">Token MCP</label>
              <div className="flex space-x-2">
                <div className="flex-1 p-2 bg-muted rounded border font-mono text-sm break-all">
                  {newTokenData}
                </div>
                <Button onClick={handleCopyToken} size="sm">
                  <Copy className="h-4 w-4" />
                </Button>
              </div>
            </div>
          </div>
          
          <DialogFooter>
            <Button onClick={handleCloseTokenDialog} className="w-full">
              Entendido, Token Salvo
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </>
  )
}