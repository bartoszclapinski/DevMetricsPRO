# Sprint 0: Environment & Project Setup 🚀

**Duration**: 3-5 days  
**Goal**: Development environment ready, project structure created, CI/CD pipeline working  
**Commitment**: 2-3 hours/day  

---

## 📋 Overview

This sprint focuses on setting up everything needed to start development. Each phase builds on the previous one and includes verification steps.

---

## Phase 0.1: Development Environment Setup (Day 1)

### Step 1.1: Install Core Tools
- [ ] **Install .NET 9 SDK**
  - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
  - Verify: `dotnet --version` should show 9.0.x
  - Test: `dotnet new list` to see available templates

- [ ] **Install Visual Studio 2022 or VS Code**
  - **VS 2022 (Recommended)**:
    - Community Edition (free)
    - Workloads: ASP.NET and web development, .NET desktop development
  - **VS Code Alternative**:
    - Extensions: C#, C# Dev Kit, .NET Extension Pack
    - Install: `dotnet tool install --global dotnet-ef`

- [ ] **Install Git**
  - Download from: https://git-scm.com/
  - Configure: 
    ```bash
    git config --global user.name "Your Name"
    git config --global user.email "your.email@example.com"
    ```

- [ ] **Install Docker Desktop**
  - Download from: https://www.docker.com/products/docker-desktop
  - Start Docker Desktop
  - Verify: `docker --version` and `docker-compose --version`

**✅ Verification**: All commands run successfully, Docker is running

---

### Step 1.2: Install Database Tools
- [ ] **Install PostgreSQL Client Tools**
  - Option 1: pgAdmin 4 (GUI)
  - Option 2: Azure Data Studio with PostgreSQL extension
  - Option 3: Command line only (psql)

- [ ] **Install Redis Insight** (Optional but recommended)
  - Download from: https://redis.com/redis-enterprise/redis-insight/
  - Visual tool for Redis debugging

**✅ Verification**: Can connect to databases once they're running

---

### Step 1.3: Setup Docker Containers for Local Development
- [ ] **Create docker-compose.yml for dev environment**
  ```yaml
  version: '3.8'
  services:
    postgres:
      image: postgres:16-alpine
      container_name: devmetrics-postgres
      environment:
        POSTGRES_USER: devmetrics
        POSTGRES_PASSWORD: Dev123456!
        POSTGRES_DB: devmetrics_dev
      ports:
        - "5432:5432"
      volumes:
        - postgres_data:/var/lib/postgresql/data
      healthcheck:
        test: ["CMD-SHELL", "pg_isready -U devmetrics"]
        interval: 10s
        timeout: 5s
        retries: 5

    redis:
      image: redis:7-alpine
      container_name: devmetrics-redis
      ports:
        - "6379:6379"
      volumes:
        - redis_data:/data
      healthcheck:
        test: ["CMD", "redis-cli", "ping"]
        interval: 10s
        timeout: 5s
        retries: 5

  volumes:
    postgres_data:
    redis_data:
  ```

- [ ] **Start containers**
  ```bash
  docker-compose up -d
  docker-compose ps  # Verify both are healthy
  ```

- [ ] **Test PostgreSQL connection**
  ```bash
  docker exec -it devmetrics-postgres psql -U devmetrics -d devmetrics_dev
  # In psql: SELECT version();
  # Exit: \q
  ```

- [ ] **Test Redis connection**
  ```bash
  docker exec -it devmetrics-redis redis-cli
  # In redis-cli: PING
  # Should return: PONG
  # Exit: exit
  ```

**✅ Verification**: Both containers running, connections working

---

## Phase 0.2: GitHub Setup (Day 1-2)

### Step 2.1: Create GitHub Repository
- [ ] **Create new repository**
  - Name: `DevMetricsPRO`
  - Description: "Real-time Developer Analytics Dashboard - .NET 9 + Blazor"
  - Visibility: Public (for portfolio) or Private
  - Add README: Yes
  - Add .gitignore: VisualStudio
  - License: MIT (recommended for portfolio)

- [ ] **Clone repository locally**
  ```bash
  cd F:\Repos
  git clone https://github.com/YOUR_USERNAME/DevMetricsPRO.git
  cd DevMetricsPRO
  ```

- [ ] **Copy existing .ai folder**
  ```bash
  # The .ai folder with all documentation
  # Should already be there based on current setup
  ```

**✅ Verification**: Repository cloned, .ai folder present

---

### Step 2.2: Setup GitHub Projects Board
- [ ] **Create new Project (Beta)**
  - Type: Board
  - Name: "DevMetrics Pro Development"

- [ ] **Create columns**:
  - 📋 Backlog
  - 🔜 Ready
  - 🏃 In Progress
  - 🧪 Testing
  - ✅ Done
  - 🚫 Blocked

- [ ] **Add initial issues from Sprint 0 & 1**
  - Create issues for each phase
  - Label them: `sprint-0`, `setup`, etc.

**✅ Verification**: Project board created with initial tasks

---

### Step 2.3: Create GitHub OAuth App (for future GitHub integration)
- [ ] **Create OAuth App**
  - Go to: Settings > Developer settings > OAuth Apps > New OAuth App
  - Application name: "DevMetrics Pro (Dev)"
  - Homepage URL: `http://localhost:5000`
  - Authorization callback URL: `http://localhost:5000/signin-github`
  - Register application

- [ ] **Save credentials**
  - Client ID: Save to password manager
  - Generate Client Secret: Save to password manager
  - ⚠️ Don't commit these to Git!

**✅ Verification**: OAuth app created, credentials saved securely

---

## Phase 0.3: Solution Structure Creation (Day 2)

### Step 3.1: Create Clean Architecture Solution
- [ ] **Navigate to project root**
  ```bash
  cd F:\Repos\DevMetricsPRO
  ```

- [ ] **Create solution and folders**
  ```bash
  # Create solution
  dotnet new sln -n DevMetricsPro

  # Create src folder
  mkdir src
  cd src
  ```

- [ ] **Create Core layer** (Domain entities, interfaces)
  ```bash
  dotnet new classlib -n DevMetricsPro.Core -f net9.0
  dotnet sln ../DevMetricsPro.sln add DevMetricsPro.Core
  ```

- [ ] **Create Application layer** (Business logic, DTOs, interfaces)
  ```bash
  dotnet new classlib -n DevMetricsPro.Application -f net9.0
  dotnet sln ../DevMetricsPro.sln add DevMetricsPro.Application
  
  # Add reference to Core
  cd DevMetricsPro.Application
  dotnet add reference ../DevMetricsPro.Core
  cd ..
  ```

- [ ] **Create Infrastructure layer** (Data access, external services)
  ```bash
  dotnet new classlib -n DevMetricsPro.Infrastructure -f net9.0
  dotnet sln ../DevMetricsPro.sln add DevMetricsPro.Infrastructure
  
  # Add references
  cd DevMetricsPro.Infrastructure
  dotnet add reference ../DevMetricsPro.Core
  dotnet add reference ../DevMetricsPro.Application
  cd ..
  ```

- [ ] **Create Web/Blazor Server project** (Presentation layer)
  ```bash
  dotnet new blazor -n DevMetricsPro.Web -f net9.0 --interactivity Server
  dotnet sln ../DevMetricsPro.sln add DevMetricsPro.Web
  
  # Add references
  cd DevMetricsPro.Web
  dotnet add reference ../DevMetricsPro.Application
  dotnet add reference ../DevMetricsPro.Infrastructure
  cd ..
  ```

**✅ Verification**: Run `dotnet build` from solution root - should build successfully

---

### Step 3.2: Create Test Projects
- [ ] **Create test folder and projects**
  ```bash
  cd ..
  mkdir tests
  cd tests

  # Unit tests for Core
  dotnet new xunit -n DevMetricsPro.Core.Tests -f net9.0
  dotnet sln ../DevMetricsPro.sln add DevMetricsPro.Core.Tests
  cd DevMetricsPro.Core.Tests
  dotnet add reference ../../src/DevMetricsPro.Core
  dotnet add package FluentAssertions
  dotnet add package Moq
  cd ..

  # Unit tests for Application
  dotnet new xunit -n DevMetricsPro.Application.Tests -f net9.0
  dotnet sln ../DevMetricsPro.sln add DevMetricsPro.Application.Tests
  cd DevMetricsPro.Application.Tests
  dotnet add reference ../../src/DevMetricsPro.Application
  dotnet add package FluentAssertions
  dotnet add package Moq
  cd ..

  # Integration tests
  dotnet new xunit -n DevMetricsPro.Integration.Tests -f net9.0
  dotnet sln ../DevMetricsPro.sln add DevMetricsPro.Integration.Tests
  cd DevMetricsPro.Integration.Tests
  dotnet add reference ../../src/DevMetricsPro.Web
  dotnet add package Microsoft.AspNetCore.Mvc.Testing
  dotnet add package FluentAssertions
  cd ..
  ```

**✅ Verification**: `dotnet test` runs (tests may be empty but should pass)

---

### Step 3.3: Setup Solution Folders
- [ ] **Create folder structure in each project**

**DevMetricsPro.Core:**
```bash
cd ../src/DevMetricsPro.Core
mkdir Entities
mkdir Enums
mkdir Exceptions
mkdir Interfaces
mkdir ValueObjects
rm Class1.cs
```

**DevMetricsPro.Application:**
```bash
cd ../DevMetricsPro.Application
mkdir DTOs
mkdir Interfaces
mkdir Services
mkdir Mappings
mkdir Validators
rm Class1.cs
```

**DevMetricsPro.Infrastructure:**
```bash
cd ../DevMetricsPro.Infrastructure
mkdir Data
mkdir Data/Configurations
mkdir Data/Migrations
mkdir Repositories
mkdir Services
mkdir Integrations
rm Class1.cs
```

**DevMetricsPro.Web:**
```bash
cd ../DevMetricsPro.Web
mkdir Components/Layout
mkdir Components/Shared
mkdir Components/Dashboard
mkdir Hubs
mkdir Extensions
```

**✅ Verification**: All folders created, default Class1.cs files removed

---

## Phase 0.4: Initial Configuration Files (Day 3)

### Step 4.1: Create .gitignore
- [ ] **Update .gitignore** (should already exist, but verify it includes):
  ```gitignore
  # Build results
  [Dd]ebug/
  [Rr]elease/
  [Bb]in/
  [Oo]bj/

  # User-specific files
  *.user
  *.suo
  *.userosscache
  *.sln.docstates

  # Visual Studio cache/options
  .vs/
  .vscode/

  # Rider
  .idea/

  # User secrets
  **/appsettings.Development.json
  **/appsettings.Local.json
  secrets.json

  # Database
  *.db
  *.db-shm
  *.db-wal

  # Docker
  docker-compose.override.yml

  # OS files
  .DS_Store
  Thumbs.db
  ```

**✅ Verification**: Sensitive files won't be committed

---

### Step 4.2: Create EditorConfig
- [ ] **Create .editorconfig** at solution root:
  ```editorconfig
  root = true

  [*]
  charset = utf-8
  indent_style = space
  indent_size = 4
  insert_final_newline = true
  trim_trailing_whitespace = true

  [*.cs]
  # Naming conventions
  dotnet_naming_rule.interface_should_be_begins_with_i.severity = warning
  dotnet_naming_rule.interface_should_be_begins_with_i.symbols = interface
  dotnet_naming_rule.interface_should_be_begins_with_i.style = begins_with_i

  # Code style rules
  csharp_prefer_braces = true:warning
  csharp_prefer_simple_using_statement = true:suggestion
  csharp_style_namespace_declarations = file_scoped:warning

  [*.{js,ts,jsx,tsx,css,scss,json,yml,yaml}]
  indent_size = 2

  [*.razor]
  indent_size = 4
  ```

**✅ Verification**: Consistent code formatting across solution

---

### Step 4.3: Create Directory.Build.props
- [ ] **Create Directory.Build.props** at src/ level:
  ```xml
  <Project>
    <PropertyGroup>
      <TargetFramework>net9.0</TargetFramework>
      <Nullable>enable</Nullable>
      <ImplicitUsings>enable</ImplicitUsings>
      <LangVersion>latest</LangVersion>
      <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
      <WarningsAsErrors>CS8600;CS8602;CS8603</WarningsAsErrors>
    </PropertyGroup>

    <ItemGroup>
      <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.0" />
    </ItemGroup>
  </Project>
  ```

**✅ Verification**: Builds with consistent settings

---

### Step 4.4: Create README.md
- [ ] **Update/Create README.md** at root:
  ```markdown
  # DevMetrics Pro 📊

  Real-time Developer Analytics Dashboard built with .NET 9 and Blazor

  ## 🚀 Features (MVP)
  - Real-time developer metrics dashboard
  - GitHub integration (commits, PRs, issues)
  - Activity heatmap (GitHub-style)
  - Team leaderboard
  - Live updates with SignalR

  ## 🛠 Tech Stack
  - .NET 9
  - Blazor Server
  - PostgreSQL + TimescaleDB
  - Redis
  - SignalR
  - Hangfire

  ## 🏗 Architecture
  Clean Architecture with:
  - Core (Domain)
  - Application (Business Logic)
  - Infrastructure (Data Access)
  - Web (Presentation - Blazor)

  ## 🚦 Getting Started

  ### Prerequisites
  - .NET 9 SDK
  - Docker Desktop
  - Git

  ### Run Locally
  ```bash
  # 1. Start databases
  docker-compose up -d

  # 2. Run migrations
  dotnet ef database update -p src/DevMetricsPro.Infrastructure -s src/DevMetricsPro.Web

  # 3. Run application
  dotnet run --project src/DevMetricsPro.Web
  ```

  ## 📝 Development Status
  - [x] Sprint 0: Setup ✅
  - [ ] Sprint 1: Foundation & Auth (In Progress)
  - [ ] Sprint 2: GitHub Integration
  - [ ] Sprint 3: Dashboard & Real-time
  - [ ] Sprint 4: Production Ready

  ## 📚 Documentation
  See `.ai/` folder for:
  - Project Requirements (prd.md)
  - Overall Plan (sprints/overall-plan.md)
  - Sprint Plans (sprints/sprint0-detailed-plan.md, etc.)

  ## 👨‍💻 Author
  Bartosz Clapiński
  ```

**✅ Verification**: README looks good on GitHub

---

## Phase 0.5: CI/CD Pipeline Setup (Day 3-4)

### Step 5.1: Create GitHub Actions Workflow
- [ ] **Create .github/workflows/dotnet-ci.yml**:
  ```yaml
  name: .NET CI

  on:
    push:
      branches: [ main, develop ]
    pull_request:
      branches: [ main, develop ]

  jobs:
    build:
      runs-on: ubuntu-latest

      services:
        postgres:
          image: postgres:16-alpine
          env:
            POSTGRES_USER: devmetrics
            POSTGRES_PASSWORD: Dev123456!
            POSTGRES_DB: devmetrics_test
          ports:
            - 5432:5432
          options: >-
            --health-cmd pg_isready
            --health-interval 10s
            --health-timeout 5s
            --health-retries 5

        redis:
          image: redis:7-alpine
          ports:
            - 6379:6379
          options: >-
            --health-cmd "redis-cli ping"
            --health-interval 10s
            --health-timeout 5s
            --health-retries 5

      steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'

      - name: Restore dependencies
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore --configuration Release

      - name: Test
        run: dotnet test --no-build --configuration Release --verbosity normal --collect:"XPlat Code Coverage"

      - name: Upload coverage reports
        uses: codecov/codecov-action@v4
        with:
          files: '**/coverage.cobertura.xml'
          fail_ci_if_error: false
  ```

- [ ] **Commit and push**
  ```bash
  git add .
  git commit -m "feat: setup CI/CD pipeline"
  git push origin main
  ```

- [ ] **Verify workflow runs on GitHub**
  - Go to Actions tab
  - Should see "NET CI" workflow running

**✅ Verification**: CI pipeline passes (green checkmark)

---

### Step 5.2: Add Status Badges to README
- [ ] **Add badges to README.md**:
  ```markdown
  # DevMetrics Pro 📊

  ![.NET CI](https://github.com/YOUR_USERNAME/DevMetricsPRO/workflows/.NET%20CI/badge.svg)
  ![.NET Version](https://img.shields.io/badge/.NET-9.0-512BD4)
  ![License](https://img.shields.io/github/license/YOUR_USERNAME/DevMetricsPRO)
  ```

**✅ Verification**: Badges show on GitHub

---

## Phase 0.6: Development Tools Setup (Day 4-5)

### Step 6.1: User Secrets Configuration
- [ ] **Initialize user secrets for Web project**
  ```bash
  cd src/DevMetricsPro.Web
  dotnet user-secrets init
  dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=devmetrics_dev;Username=devmetrics;Password=Dev123456!"
  dotnet user-secrets set "ConnectionStrings:Redis" "localhost:6379"
  dotnet user-secrets set "GitHub:ClientId" "YOUR_GITHUB_CLIENT_ID"
  dotnet user-secrets set "GitHub:ClientSecret" "YOUR_GITHUB_CLIENT_SECRET"
  dotnet user-secrets set "Jwt:Key" "YourSuperSecretKeyThatIsAtLeast32CharactersLong!"
  dotnet user-secrets set "Jwt:Issuer" "https://localhost:5001"
  dotnet user-secrets set "Jwt:Audience" "https://localhost:5001"
  ```

**✅ Verification**: Secrets stored securely (not in Git)

---

### Step 6.2: Create appsettings Templates
- [ ] **Update src/DevMetricsPro.Web/appsettings.json**:
  ```json
  {
    "Logging": {
      "LogLevel": {
        "Default": "Information",
        "Microsoft.AspNetCore": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning"
      }
    },
    "AllowedHosts": "*",
    "ConnectionStrings": {
      "DefaultConnection": "REPLACE_WITH_USER_SECRETS",
      "Redis": "REPLACE_WITH_USER_SECRETS"
    },
    "Jwt": {
      "Key": "REPLACE_WITH_USER_SECRETS",
      "Issuer": "REPLACE_WITH_USER_SECRETS",
      "Audience": "REPLACE_WITH_USER_SECRETS",
      "ExpirationMinutes": 60
    },
    "GitHub": {
      "ClientId": "REPLACE_WITH_USER_SECRETS",
      "ClientSecret": "REPLACE_WITH_USER_SECRETS",
      "ApiBaseUrl": "https://api.github.com"
    }
  }
  ```

- [ ] **Create appsettings.Development.json** (add to .gitignore):
  ```json
  {
    "Logging": {
      "LogLevel": {
        "Default": "Debug",
        "Microsoft.AspNetCore": "Information",
        "Microsoft.EntityFrameworkCore": "Information"
      }
    },
    "DetailedErrors": true
  }
  ```

**✅ Verification**: Configuration structure in place

---

## Phase 0.7: Documentation & Wrap-up (Day 5)

### Step 7.1: Create Sprint Log
- [ ] **Create .ai/sprints/sprint0-log.md**:
  ```markdown
  # Sprint 0 - Setup & Preparation - Log

  **Start Date**: [YYYY-MM-DD]  
  **End Date**: [YYYY-MM-DD]  
  **Status**: ✅ Completed  

  ## Goals
  - [x] Development environment setup
  - [x] Project structure created
  - [x] CI/CD pipeline working
  - [x] Documentation complete

  ## What Was Done
  - Installed all required tools (.NET 9, Docker, Git)
  - Created Clean Architecture solution
  - Setup PostgreSQL and Redis with Docker
  - Created GitHub repository and Actions workflow
  - Configured user secrets
  - Created initial documentation

  ## Challenges
  - None significant

  ## Metrics
  - Commits: X
  - Time spent: ~X hours
  - Test coverage: N/A (no code yet)

  ## Next Sprint Preview
  Sprint 1 will focus on:
  - Core domain entities
  - Database setup with EF Core
  - Repository pattern
  - Basic authentication
  ```

**✅ Verification**: Log created

---

### Step 7.2: Commit All Changes
- [ ] **Review all changes**
  ```bash
  cd F:\Repos\DevMetricsPRO
  git status
  ```

- [ ] **Commit in logical chunks**
  ```bash
  # Commit solution structure
  git add *.sln src/ tests/
  git commit -m "feat: initial solution structure with Clean Architecture"

  # Commit Docker setup
  git add docker-compose.yml
  git commit -m "feat: add docker-compose for PostgreSQL and Redis"

  # Commit configuration
  git add .editorconfig Directory.Build.props .gitignore
  git commit -m "chore: add configuration files"

  # Commit documentation
  git add README.md .ai/
  git commit -m "docs: add project documentation and sprint plans"

  # Push all
  git push origin main
  ```

**✅ Verification**: All committed, CI passes

---

### Step 7.3: Create Sprint 0 Release Tag
- [ ] **Tag the completion**
  ```bash
  git tag -a v0.1-sprint0 -m "Sprint 0: Setup complete"
  git push origin v0.1-sprint0
  ```

**✅ Verification**: Tag visible on GitHub

---

## 🎯 Sprint 0 Success Criteria

- [x] All development tools installed and working
- [x] Docker containers running (PostgreSQL, Redis)
- [x] Solution structure created with Clean Architecture
- [x] Test projects setup
- [x] CI/CD pipeline green
- [x] User secrets configured
- [x] Documentation complete
- [x] First commits pushed to GitHub

---

## 📊 Estimated Time Breakdown

| Phase | Estimated Time | Actual Time |
|-------|---------------|-------------|
| 0.1 - Dev Environment | 2-3 hours | |
| 0.2 - GitHub Setup | 1 hour | |
| 0.3 - Solution Structure | 2 hours | |
| 0.4 - Configuration | 1 hour | |
| 0.5 - CI/CD | 2 hours | |
| 0.6 - Dev Tools | 1 hour | |
| 0.7 - Documentation | 1 hour | |
| **Total** | **10-12 hours** | |

---

## 🚀 Ready for Sprint 1!

Once all checkboxes are marked, you're ready to start Sprint 1: Foundation & Architecture!

Next up:
- Core domain entities
- Entity Framework Core setup
- Repository pattern implementation
- Basic authentication

