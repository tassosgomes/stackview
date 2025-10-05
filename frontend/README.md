# StackShare Frontend

## Overview

Modern React application built with TypeScript, Vite, Tailwind CSS, and ShadCN/UI components for the StackShare platform.

## Tech Stack

- **Framework**: React 18+ with TypeScript
- **Build Tool**: Vite 7.x
- **Styling**: Tailwind CSS 3.4+ 
- **UI Components**: ShadCN/UI
- **Routing**: React Router Dom
- **State Management**: React Query (TanStack Query)
- **HTTP Client**: Axios
- **Form Handling**: React Hook Form + Zod validation
- **Markdown Rendering**: React Markdown

## Features

- 🎨 Modern UI with Tailwind CSS and ShadCN components
- 🚀 Fast development with Vite HMR
- 📱 Responsive design
- 🔍 Type-safe development with TypeScript
- 🔄 Smart data fetching with React Query
- 🛣️ Client-side routing
- 📝 Markdown support for stack descriptions
- 🔐 JWT authentication ready
- 🌙 Dark/Light theme support (via ShadCN)

## Project Structure

```
src/
├── components/          # Reusable UI components
│   ├── ui/             # ShadCN/UI components
│   └── layout.tsx      # App layout component
├── features/           # Feature-specific components and logic
├── hooks/              # Custom React hooks
├── lib/                # Utility functions and configurations
│   ├── api-client.ts   # Axios configuration
│   ├── react-query.tsx # React Query setup
│   └── utils.ts        # Utility functions
├── pages/              # Page components
│   ├── home.tsx        # Landing page
│   ├── login.tsx       # Authentication
│   └── dashboard.tsx   # User dashboard
└── services/           # API service functions
```

## Getting Started

### Prerequisites

- Node.js 18+ and npm

### Installation

```bash
# Install dependencies
npm install

# Copy environment variables
cp .env.example .env.local

# Start development server
npm run dev
```

### Available Scripts

```bash
# Development
npm run dev          # Start development server

# Building
npm run build        # Build for production
npm run preview      # Preview production build

# Linting
npm run lint         # Run ESLint
```

## Environment Variables

```bash
# .env.local
VITE_API_BASE_URL=http://localhost:5096/api
VITE_APP_NAME=StackShare
VITE_APP_VERSION=1.0.0
```

## Development

### Adding New Components

```bash
# Add ShadCN components
npx shadcn@latest add <component-name>

# Example: Add a dialog component
npx shadcn@latest add dialog
```

### Routing

Routes are configured in `src/App.tsx`. Add new routes in the Routes component:

```tsx
<Route path="/new-page" element={<NewPage />} />
```

### API Integration

Use the configured Axios client in `src/lib/api-client.ts`:

```tsx
import apiClient from '@/lib/api-client'

// In a service file
export const fetchStacks = async () => {
  const response = await apiClient.get('/stacks')
  return response.data
}
```

### State Management

Use React Query for server state:

```tsx
import { useQuery } from '@tanstack/react-query'

function MyComponent() {
  const { data, isLoading } = useQuery({
    queryKey: ['stacks'],
    queryFn: fetchStacks,
  })
  
  // Component logic
}
```

## Deployment

The application builds to static files that can be served by any web server:

```bash
npm run build
# Files are generated in the dist/ directory
```

## Integration with Backend

- API Base URL: Configurable via `VITE_API_BASE_URL`
- Authentication: JWT tokens stored in localStorage
- Automatic token refresh and logout on 401 responses

## Contributing

1. Follow React functional component patterns
2. Use TypeScript for type safety
3. Follow Tailwind CSS utility-first approach
4. Use ShadCN components when possible
5. Implement React Query for data fetching
6. Follow the established folder structure

## Browser Support

- Chrome/Edge 90+
- Firefox 88+
- Safari 14+

## Performance

- Code splitting with React Router
- Optimized bundle with Vite
- Tree shaking enabled
- CSS purging in production
