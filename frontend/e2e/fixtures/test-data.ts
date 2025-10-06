export interface TestUser {
  email: string;
  password: string;
  name: string;
}

export interface TestStack {
  name: string;
  type: string;
  description: string;
  technologies: string[];
  isPublic: boolean;
}

export const TEST_USERS = {
  validUser: {
    email: 'test@example.com',
    password: 'Password123!',
    name: 'Test User'
  } as TestUser,
  
  invalidUser: {
    email: 'invalid@example.com',
    password: 'wrongpassword',
    name: 'Invalid User'
  } as TestUser
};

export const TEST_STACKS = {
  frontendStack: {
    name: 'React E-commerce Frontend',
    type: 'Frontend',
    description: `# React E-commerce Frontend

Este é um stack frontend moderno para uma aplicação de e-commerce.

## Tecnologias Utilizadas

- **React 18**: Framework principal
- **TypeScript**: Tipagem estática
- **Tailwind CSS**: Estilização
- **Vite**: Build tool

## Características

- Interface responsiva
- Performance otimizada
- SEO friendly
- Acessibilidade (WCAG 2.1)

## Estrutura

\`\`\`
src/
├── components/
├── pages/
├── hooks/
└── utils/
\`\`\`
`,
    technologies: ['React', 'TypeScript', 'Tailwind CSS', 'Vite'],
    isPublic: true
  } as TestStack,

  backendStack: {
    name: 'Node.js API Server',
    type: 'Backend', 
    description: `# Node.js API Server

API REST robusta construída com Node.js e Express.

## Stack Tecnológico

- Node.js 18+
- Express.js
- PostgreSQL
- JWT Authentication

## Features

- CRUD completo
- Autenticação segura
- Logs estruturados
- Testes automatizados
`,
    technologies: ['Node.js', 'Express.js', 'PostgreSQL', 'JWT'],
    isPublic: true
  } as TestStack,

  mobileStack: {
    name: 'Flutter Mobile App',
    type: 'Mobile',
    description: `# Flutter Mobile App

Aplicativo mobile multiplataforma desenvolvido em Flutter.

## Tecnologias

- Flutter
- Dart
- Firebase
- Provider

## Plataformas

- iOS
- Android
`,
    technologies: ['Flutter', 'Dart', 'Firebase', 'Provider'],
    isPublic: false
  } as TestStack
};

export const TEST_TECHNOLOGIES = [
  'React',
  'Vue.js',
  'Angular',
  'Node.js',
  'Express.js',
  'TypeScript',
  'JavaScript',
  'Python',
  'Django',
  'FastAPI',
  'PostgreSQL',
  'MongoDB',
  'Redis',
  'Docker',
  'Kubernetes'
];

export const STACK_TYPES = [
  'Frontend',
  'Backend', 
  'Mobile',
  'DevOps',
  'Data',
  'Testing'
];

export class TestDataHelper {
  static generateRandomStackName(): string {
    const adjectives = ['Modern', 'Scalable', 'Robust', 'Efficient', 'Innovative'];
    const nouns = ['Stack', 'Platform', 'System', 'Application', 'Solution'];
    const adj = adjectives[Math.floor(Math.random() * adjectives.length)];
    const noun = nouns[Math.floor(Math.random() * nouns.length)];
    const timestamp = Date.now().toString().slice(-4);
    return `${adj} ${noun} ${timestamp}`;
  }

  static generateRandomEmail(): string {
    const timestamp = Date.now().toString().slice(-6);
    return `test${timestamp}@example.com`;
  }

  static getRandomTechnologies(count: number = 3): string[] {
    const shuffled = [...TEST_TECHNOLOGIES].sort(() => 0.5 - Math.random());
    return shuffled.slice(0, count);
  }

  static getRandomStackType(): string {
    return STACK_TYPES[Math.floor(Math.random() * STACK_TYPES.length)];
  }
}