using StackShare.Domain.Entities;
using StackShare.Infrastructure.Data;

namespace StackShare.Infrastructure.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(StackShareDbContext context)
    {
        // Seed technologies se não existirem
        if (!context.Technologies.Any())
        {
            var technologies = new List<Technology>
            {
                // Frontend
                new Technology { Id = Guid.NewGuid(), Name = "React", Description = "A JavaScript library for building user interfaces", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Vue.js", Description = "The Progressive JavaScript Framework", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Angular", Description = "Platform for building mobile and desktop web applications", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Next.js", Description = "The React Framework for the Web", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "TypeScript", Description = "TypeScript is JavaScript with syntax for types", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Tailwind CSS", Description = "A utility-first CSS framework", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Bootstrap", Description = "Build fast, responsive sites with Bootstrap", IsPreRegistered = true },
                
                // Backend
                new Technology { Id = Guid.NewGuid(), Name = "ASP.NET Core", Description = "Cross-platform .NET framework for building web applications", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Node.js", Description = "JavaScript runtime built on Chrome's V8 JavaScript engine", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Express.js", Description = "Fast, unopinionated, minimalist web framework for Node.js", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Spring Boot", Description = "Framework to create stand-alone, production-grade Spring applications", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Django", Description = "High-level Python web framework", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Flask", Description = "Lightweight WSGI web application framework", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "FastAPI", Description = "Modern, fast web framework for building APIs with Python", IsPreRegistered = true },
                
                // Databases
                new Technology { Id = Guid.NewGuid(), Name = "PostgreSQL", Description = "Advanced open source relational database", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "MySQL", Description = "World's most popular open source database", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "MongoDB", Description = "Document-based, distributed database", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Redis", Description = "In-memory data structure store", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "SQLite", Description = "Self-contained, serverless, zero-configuration database engine", IsPreRegistered = true },
                
                // Mobile
                new Technology { Id = Guid.NewGuid(), Name = "React Native", Description = "Framework for building native mobile apps using React", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Flutter", Description = "UI toolkit for building compiled applications for mobile, web, and desktop", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Xamarin", Description = "Platform for building iOS and Android apps with .NET", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Swift", Description = "Programming language for iOS, macOS, watchOS, and tvOS", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Kotlin", Description = "Modern programming language for Android development", IsPreRegistered = true },
                
                // DevOps
                new Technology { Id = Guid.NewGuid(), Name = "Docker", Description = "Platform for developing, shipping, and running applications in containers", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Kubernetes", Description = "Container orchestration system", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "AWS", Description = "Amazon Web Services cloud computing platform", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Azure", Description = "Microsoft's cloud computing platform", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "GitHub Actions", Description = "Automate, customize, and execute software development workflows", IsPreRegistered = true },
                
                // Programming Languages
                new Technology { Id = Guid.NewGuid(), Name = "C#", Description = "Modern, object-oriented programming language", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "JavaScript", Description = "High-level, interpreted programming language", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Python", Description = "High-level, interpreted programming language", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Java", Description = "Object-oriented programming language", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Go", Description = "Open source programming language developed by Google", IsPreRegistered = true },
                new Technology { Id = Guid.NewGuid(), Name = "Rust", Description = "Systems programming language focused on safety and performance", IsPreRegistered = true }
            };

            await context.Technologies.AddRangeAsync(technologies);
            await context.SaveChangesAsync();
        }
    }
}