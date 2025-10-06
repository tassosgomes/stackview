export interface McpToken {
  id: string
  createdAt: string
  isRevoked: boolean
}

export interface McpTokenResponse {
  id: string
  createdAt: string
  isRevoked: boolean
}

export interface CreateMcpTokenResponse {
  rawToken: string
  id: string
  createdAt: string
}

// No additional fields needed for basic token creation - using void instead of empty interface