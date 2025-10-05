export interface Stack {
  id: string
  name: string
  description: string
  type: StackType
  isPublic: boolean
  createdAt: string
  updatedAt: string
  technologies: Technology[]
  userId: string
}

export interface StackSummary {
  id: string
  name: string
  type: StackType
  isPublic: boolean
  createdAt: string
  technologyCount: number
}

export interface Technology {
  id: string
  name: string
  category: TechnologyCategory
}

export type StackType = 
  | 'Frontend'
  | 'Backend'
  | 'Mobile'
  | 'DevOps'
  | 'Data'
  | 'Testing'

export type TechnologyCategory = 
  | 'Language'
  | 'Framework'
  | 'Library'
  | 'Tool'
  | 'Database'
  | 'Cloud'

export interface CreateStackRequest {
  name: string
  description: string
  type: StackType
  isPublic: boolean
  technologyIds: string[]
}

export type UpdateStackRequest = CreateStackRequest

export interface PagedList<T> {
  items: T[]
  totalCount: number
  page: number
  pageSize: number
  totalPages: number
}