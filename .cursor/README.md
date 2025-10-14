# Cursor Rules for DevMetrics Pro

This directory contains comprehensive coding rules and conventions for the DevMetrics Pro project.

## 📚 Rule Files

### Main Configuration
- **`.cursorrules`** (in project root) - Main rules file with project overview and quick reference

### Detailed Rule Files
Each `.mdc` file contains in-depth rules for a specific area:

| File | Purpose | Key Topics |
|------|---------|------------|
| **architecture.mdc** | Clean Architecture principles | Layer responsibilities, dependency rules, file organization |
| **dotnet-conventions.mdc** | .NET & C# standards | C# 12 features, naming, async patterns, DI |
| **blazor-rules.mdc** | Blazor development | Components, lifecycle, SignalR, MudBlazor |
| **database-rules.mdc** | EF Core & PostgreSQL | Entities, migrations, queries, performance |
| **testing-rules.mdc** | Testing standards | xUnit, FluentAssertions, mocking, coverage |

## 🎯 How to Use These Rules

### For AI Assistants (Cursor)
The `.cursorrules` file is automatically loaded by Cursor AI. For detailed guidance on specific topics, refer to the appropriate `.mdc` file.

### For Developers
1. Read `.cursorrules` for project overview
2. Refer to specific `.mdc` files when working in that area:
   - Adding a new entity? Read `architecture.mdc` and `database-rules.mdc`
   - Creating a Blazor component? Read `blazor-rules.mdc`
   - Writing tests? Read `testing-rules.mdc`

## 🏗️ Architecture Quick Reference

### Clean Architecture Layers
```
Core (Domain)
  ↑
Application (Business Logic)
  ↑
Infrastructure (Data & External Services)
  ↑
Web (UI & API)
```

**Rule**: Dependencies ONLY point upward (toward Core)

### Key Principles
1. **Core** has no dependencies on other layers
2. **Application** depends only on Core
3. **Infrastructure** implements interfaces from Application/Core
4. **Web** orchestrates everything

## 📝 Quick Standards

### C# Conventions
- File-scoped namespaces: `namespace MyNamespace;`
- Nullable reference types: `<Nullable>enable</Nullable>`
- Async methods: Always include `CancellationToken`
- Private fields: `_camelCase`
- Public properties: `PascalCase`

### Blazor Conventions
- Render mode: `@rendermode InteractiveServer`
- Component parameters: `[Parameter] public required Type Prop { get; set; }`
- Event callbacks: `[Parameter] public EventCallback OnClick { get; set; }`
- Lifecycle: Use `OnInitializedAsync` for data loading

### Database Conventions
- Fluent API in separate configuration classes
- Always use async methods with `CancellationToken`
- Use `AsNoTracking()` for read-only queries
- Project to DTOs in queries with `Select()`

### Testing Conventions
- Test naming: `MethodName_Scenario_ExpectedBehavior`
- Use AAA pattern: Arrange-Act-Assert
- FluentAssertions for assertions
- 80% minimum code coverage

## 🚀 Common Tasks

### Adding a New Entity
1. Create entity in `Core/Entities/`
2. Create configuration in `Infrastructure/Data/Configurations/`
3. Create DTO in `Application/DTOs/`
4. Add to `ApplicationDbContext.cs`
5. Create migration
6. Write tests

### Creating a New Blazor Component
1. Create `.razor` file in appropriate folder
2. Add `@rendermode InteractiveServer`
3. Define `[Parameter]` properties
4. Implement lifecycle methods
5. Create `.razor.css` for component-specific styles
6. Write component tests

### Adding a New Service
1. Define interface in `Application/Interfaces/`
2. Create implementation in `Application/Services/`
3. Register in `Program.cs` DI container
4. Write unit tests with mocks
5. Write integration tests

## 🔍 Decision Trees

### Where Does This Code Go?

**Is it a domain concept?** → `Core/`
- Entity? → `Core/Entities/`
- Enum? → `Core/Enums/`
- Repository interface? → `Core/Interfaces/`

**Is it business logic?** → `Application/`
- Service? → `Application/Services/`
- DTO? → `Application/DTOs/`
- Validator? → `Application/Validators/`

**Is it technical implementation?** → `Infrastructure/`
- Database? → `Infrastructure/Data/`
- Repository? → `Infrastructure/Repositories/`
- External API? → `Infrastructure/Services/`

**Is it UI or API?** → `Web/`
- Blazor component? → `Web/Components/`
- API endpoint? → `Web/Controllers/`
- SignalR? → `Web/Hubs/`

## ⚠️ Common Mistakes to Avoid

1. **❌ Referencing outer layers from inner layers**
   - Never `using Infrastructure` in Core
   - Never `using Web` in Application

2. **❌ Using sync database calls**
   - Use `ToListAsync()`, not `ToList()`
   - Always pass `CancellationToken`

3. **❌ Exposing entities in APIs**
   - Use DTOs, not entities
   - Map entities to DTOs in services

4. **❌ Putting business logic in components/controllers**
   - Keep logic in Application services
   - Components/controllers orchestrate only

5. **❌ Not disposing resources**
   - Implement `IAsyncDisposable` for Blazor components
   - Dispose DbContext properly

## 📊 Quality Gates

Before committing code:
- [ ] Follows Clean Architecture rules
- [ ] Uses async/await with CancellationToken
- [ ] DTOs used for API contracts
- [ ] Proper error handling
- [ ] Tests written (80% coverage)
- [ ] No linter errors
- [ ] Proper naming conventions
- [ ] XML documentation for public APIs

## 🆘 Need Help?

1. **For architecture questions**: Read `architecture.mdc`
2. **For C# patterns**: Read `dotnet-conventions.mdc`
3. **For Blazor help**: Read `blazor-rules.mdc`
4. **For database queries**: Read `database-rules.mdc`
5. **For testing guidance**: Read `testing-rules.mdc`

## 📚 Learning Resources

### Official Documentation
- [.NET 9 Docs](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-9)
- [Blazor Docs](https://docs.microsoft.com/en-us/aspnet/core/blazor/)
- [EF Core Docs](https://docs.microsoft.com/en-us/ef/core/)
- [MudBlazor Docs](https://mudblazor.com/)

### Project Documentation
- `.ai/GETTING-STARTED.md` - Quick start guide
- `.ai/prd.md` - Product requirements
- `.ai/sprints/` - Sprint plans and logs

---

**Remember**: These rules exist to maintain consistency, quality, and scalability. When in doubt, follow Clean Architecture principles and prioritize clarity over cleverness.

