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

export interface CreateMcpTokenRequest {
  // No additional fields needed for basic token creation
}