# DevMetrics Pro 📊

> Real-time developer analytics dashboard built with .NET 9 and Blazor Server

## Build & Quality
[![.NET CI](https://github.com/bartoszclapinski/DevMetricsPRO/workflows/.NET%20CI/badge.svg)](https://github.com/bartoszclapinski/DevMetricsPRO/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Code Coverage](https://img.shields.io/badge/Coverage-80%25-brightgreen?style=flat)](https://github.com/bartoszclapinski/DevMetricsPRO)
[![PRs Welcome](https://img.shields.io/badge/PRs-Welcome-brightgreen?style=flat)](https://github.com/bartoszclapinski/DevMetricsPRO/pulls)

## Technology Stack
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat&logo=.net&logoColor=white)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor-Server-512BD4?style=flat&logo=blazor&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![C#](https://img.shields.io/badge/C%23-12-239120?style=flat&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/dotnet/csharp/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=flat&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7-DC382D?style=flat&logo=redis&logoColor=white)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Enabled-2496ED?style=flat&logo=docker&logoColor=white)](https://www.docker.com/)

## Integrations
[![GitHub](https://img.shields.io/badge/GitHub-Integrated-181717?style=flat&logo=github&logoColor=white)](https://github.com)
[![GitLab](https://img.shields.io/badge/GitLab-Planned-FC6D26?style=flat&logo=gitlab&logoColor=white)](https://gitlab.com)
[![Jira](https://img.shields.io/badge/Jira-Planned-0052CC?style=flat&logo=jira&logoColor=white)](https://www.atlassian.com/software/jira)

## Architecture & Design
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean-blue?style=flat)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
[![MudBlazor](https://img.shields.io/badge/UI-MudBlazor-594AE2?style=flat)](https://mudblazor.com/)

## 🚀 About

DevMetrics Pro is a comprehensive developer analytics platform that provides real-time insights into team productivity, code quality, and project health. It integrates with popular development tools to give you a unified view of your team's performance.

## 🎨 UI Design Prototype

![DevMetrics Pro Dashboard](.ai/design/design-prototype-ss.png)

*The dashboard will feature real-time metrics, interactive charts, and team leaderboards.*

### Key Features

**✅ Implemented:**
- 🔐 **Authentication** - ASP.NET Core Identity with JWT
- 🔗 **GitHub OAuth** - Connect GitHub accounts securely
- 📦 **Data Sync** - Automatic sync of repos, commits, and PRs
- 🔄 **Background Jobs** - Hangfire for automated syncing
- 📊 **Metrics Calculation** - Developer productivity metrics
- 🗄️ **Caching** - Redis for performance optimization
- 🔒 **Security** - Rate limiting, CORS, security headers
- 📈 **Charts** - Chart.js integration with JSInterop

**🚀 In Progress (Sprint 3 - 80% Complete!):**
- ✅ **Interactive Charts** - Commit activity line chart, PR statistics bar chart
- ✅ **Contribution Heatmap** - GitHub-style activity visualization
- ✅ **Real-time Updates** - SignalR for live dashboard updates
- ✅ **Team Leaderboards** - Developer productivity rankings
- ✅ **Advanced Metrics** - PR review time, code velocity analysis
- 🔄 **Time Range Filters** - Global dashboard filtering (next)

**📅 Planned:**
- 🔄 **Multi-Platform** - GitLab and Jira integration
- 📉 **Trend Analysis** - Historical metrics tracking
- 🎯 **Sprint Metrics** - Velocity and burndown charts
- 🎨 **Custom Dashboards** - User-configurable layouts

## 🛠 Tech Stack

### Backend
- **.NET 9** - Latest C# 12 features
- **Blazor Server** - Real-time interactive UI
- **ASP.NET Core Identity** - Authentication & authorization
- **Entity Framework Core 9** - ORM with PostgreSQL
- **SignalR** - Real-time communication
- **Hangfire** - Background job processing

### Database & Caching
- **PostgreSQL 16** - Primary database
- **TimescaleDB** - Time-series data optimization
- **Redis 7** - Distributed caching

### Frontend
- **MudBlazor** - Material Design component library
- **Chart.js** - Interactive charts and visualizations
- **JavaScript Interop** - Blazor-JS integration
- **SignalR** - Real-time communication (planned)

### DevOps
- **Docker** - Containerization
- **GitHub Actions** - CI/CD pipeline
- **Azure App Service** - Hosting (planned)

## 🏗 Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────┐
│         DevMetricsPro.Web           │  Presentation Layer
│         (Blazor Server)             │
├─────────────────────────────────────┤
│    DevMetricsPro.Infrastructure     │  External Services
│    (Data, APIs, Background Jobs)    │
├─────────────────────────────────────┤
│    DevMetricsPro.Application        │  Business Logic
│    (Services, DTOs, Validators)     │
├─────────────────────────────────────┤
│       DevMetricsPro.Core            │  Domain Layer
│       (Entities, Interfaces)        │
└─────────────────────────────────────┘
```

**Benefits:**
- ✅ Testable and maintainable
- ✅ Framework-independent business logic
- ✅ Clear dependency direction (inward)
- ✅ Easy to extend and modify

## 📝 Development Status

| Sprint | Status | Focus | Completion |
|--------|--------|-------|------------|
| **Sprint 0** | ✅ Complete | Environment setup, project structure | 100% |
| **Sprint 1** | ✅ Complete | Foundation, database, authentication | 100% |
| **Sprint 2** | ✅ Complete | GitHub integration, background jobs, metrics | 100% |
| **Sprint 3** | 🚀 In Progress | Dashboard, charts, real-time features | 80% |
| **Sprint 4** | 📅 Planned | Production readiness, deployment | 0% |

**Current Sprint**: Sprint 3 - Real-time Dashboard & Analytics  
**Current Phase**: Phase 3.9 - Time Range Filters (Next)  
**Last Update**: December 2, 2025

## 🚦 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### Quick Start

```bash
# Clone the repository
git clone https://github.com/bartoszclapinski/DevMetricsPRO.git
cd DevMetricsPRO

# Start databases
docker-compose up -d

# Run migrations (once solution is created)
dotnet ef database update -p src/DevMetricsPro.Infrastructure -s src/DevMetricsPro.Web

# Run the application
dotnet run --project src/DevMetricsPro.Web

# Open browser
# https://localhost:5001
```

## 📚 Documentation

Comprehensive documentation is available in the [`.ai/`](.ai/) folder:

- **[Getting Started](.ai/GETTING-STARTED.md)** - Quick start guide for developers
- **[Product Requirements](.ai/prd.md)** - Complete PRD with technical specs
- **[Sprint Plans](.ai/sprints/)** - Detailed implementation plans
- **[Design Prototype](.ai/design/design-prototype.html)** - UI/UX reference

### For Developers

- **[Cursor Rules](.cursor/)** - AI-assisted development guidelines
  - Clean Architecture principles
  - .NET 9 & C# 12 conventions
  - Blazor best practices
  - Database guidelines
  - Testing standards

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/DevMetricsPro.Application.Tests
```

**Coverage Goal:** 80% minimum

## 🔄 CI/CD

Every push and pull request triggers automated checks:

- ✅ Build verification
- ✅ Unit & integration tests
- ✅ Code quality analysis
- ✅ Security scanning
- ✅ Format verification

See [.github/README.md](.github/README.md) for details.

## 🤝 Contributing

This is a learning project, but contributions are welcome!

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Follow the coding standards in `.cursor/` folder
4. Commit your changes (`git commit -m 'feat: add amazing feature'`)
5. Push to the branch (`git push origin feature/amazing-feature`)
6. Open a Pull Request

**Commit Convention:** This project follows [Conventional Commits](https://www.conventionalcommits.org/)
- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation
- `refactor:` - Code refactoring
- `test:` - Adding tests
- `chore:` - Maintenance

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Bartosz Clapiński**

Building this project to learn:
- .NET 9 and Blazor Server
- Clean Architecture principles
- Real-time web applications with SignalR
- DevOps and CI/CD practices

## 🎯 Project Goals

This project serves multiple purposes:

1. **Learning** - Deep dive into .NET 9, Blazor, and Clean Architecture
2. **Portfolio** - Showcase modern .NET development practices
3. **Building in Public** - Document the journey from idea to production
4. **Best Practices** - Implement industry-standard patterns and principles

## 📈 Roadmap

### Phase 1: MVP (Sprints 0-4)
- [x] Project setup and documentation
- [x] CI/CD pipeline
- [x] Core foundation with authentication (Sprint 1)
- [x] GitHub OAuth integration (Sprint 2)
- [x] Repository, commits, and PR sync (Sprint 2)
- [x] Background jobs with Hangfire (Sprint 2)
- [x] Metrics calculation (Sprint 2)
- [x] Security hardening & caching (Sprint 2)
- [x] Chart.js integration (Sprint 3 - Phase 3.1) ✅
- [x] Interactive charts - line and bar (Sprint 3 - Phases 3.2-3.3) ✅
- [x] GitHub-style contribution heatmap (Sprint 3 - Phase 3.4) ✅
- [x] Team leaderboards (Sprint 3 - Phase 3.5) ✅
- [x] Real-time dashboard with SignalR (Sprint 3 - Phases 3.6-3.7) ✅
- [x] Advanced metrics (Sprint 3 - Phase 3.8) ✅
- [ ] Time range filters & polish (Sprint 3 - Phases 3.9-3.10 - In Progress)
- [ ] Production deployment (Sprint 4 - Planned)

### Phase 2: Enhancement (Sprint 5+)
- [ ] GitLab integration
- [ ] Jira integration
- [ ] Advanced analytics
- [ ] Export functionality
- [ ] Mobile responsiveness
- [ ] Performance optimization

### Phase 3: Advanced Features
- [ ] AI-powered insights
- [ ] Predictive analytics
- [ ] Custom dashboards
- [ ] API for third-party integrations

## 🌟 Acknowledgments

- [MudBlazor](https://mudblazor.com/) - Beautiful Blazor component library
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html) - Robert C. Martin (Uncle Bob)
- [.NET Team](https://github.com/dotnet) - For the amazing framework

---

⭐ **Star this repo** if you find it useful!

📧 **Questions?** Open an issue or reach out!

🚀 **Following the journey?** Watch this repo for updates!

