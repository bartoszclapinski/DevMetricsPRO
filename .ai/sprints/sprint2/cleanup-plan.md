# Sprint 2 Cleanup Plan 🧹

**Duration**: 1 week (5-7 working days)
**Goal**: Production-ready codebase with tests, performance optimization, and polish
**Started**: November 13, 2025

---

## 📋 Overview

After completing Sprint 2 (GitHub Integration & Background Jobs), this cleanup phase ensures the codebase is:
- **Tested**: Unit tests for critical services
- **Performant**: Database indexes, caching, query optimization
- **Robust**: Error handling, logging improvements
- **Clean**: Code quality and maintainability

---

## 🎯 Cleanup Phases

### Phase 2.C.1: Testing & Quality 🧪

**Goal**: Add comprehensive unit tests for metrics service

#### Tasks:
- [x] Create `MetricsCalculationServiceTests.cs` in test project
- [x] Test `CalculateMetricsForDeveloperAsync` method
- [x] Test `CalculateMetricsForAllDevelopersAsync` method
- [x] Test edge cases (no commits, no PRs, date ranges)
- [x] Test upsert logic (create vs update)
- [x] Mock dependencies (IUnitOfWork, repositories)
- [x] Achieve >80% code coverage for service

**Time Estimate**: 1 day

---

### Phase 2.C.2: Database Performance 🚀

**Goal**: Optimize database queries and add indexes

#### Task 2.C.2.1: Add Database Indexes
- [x] Add index on `Commits.DeveloperId` (verified existing)
- [x] Add index on `Commits.RepositoryId` (verified existing)
- [x] Add index on `Commits.CommittedAt` (verified existing)
- [x] Add index on `PullRequests.RepositoryId` (verified existing)
- [x] Add index on `PullRequests.AuthorId` (verified existing)
- [x] Add index on `PullRequests.Status` (verified existing)
- [x] Add index on `Metrics.DeveloperId` (verified existing)
- [x] Add index on `Metrics.MetricType` (verified existing)
- [x] Create EF Core migration for indexes (not needed after verification)
- [x] Apply migration to database (not needed after verification)

#### Task 2.C.2.2: Query Optimization
- [x] Add `.AsNoTracking()` to read-only queries in GitHubController (via repository `Query()` helper)
- [x] Add `.AsNoTracking()` to metrics calculations
- [x] Add `.AsNoTracking()` to repository/commit/PR listings
- [x] Review and optimize N+1 query issues
- [x] Use `.Include()` for eager loading where appropriate

**Time Estimate**: 1 day

---

### Phase 2.C.3: Error Handling & User Experience 🛡️

**Goal**: Improve error handling with user-friendly messages

#### Tasks:
- [x] Create custom exception types (NotFoundException, ValidationException, etc.)
- [x] Add global exception handler middleware
- [x] Return user-friendly error messages from API endpoints
- [x] Add validation to DTOs (FluentValidation)
- [x] Handle GitHub API rate limit errors gracefully
- [x] Add retry logic for transient failures (Polly)
- [x] Display error messages in Blazor components
- [x] Add error logging to Application Insights/Serilog

**Time Estimate**: 1 day

---

### Phase 2.C.4: Caching Layer 💾

**Goal**: Implement Redis caching for frequently accessed data

#### Tasks:
- [ ] Add `IDistributedCache` service registration
- [ ] Create `ICacheService` interface
- [ ] Implement `RedisCacheService`
- [ ] Cache repository lists (TTL: 5 minutes)
- [ ] Cache calculated metrics (TTL: 10 minutes)
- [ ] Cache user GitHub connection status (TTL: 1 minute)
- [ ] Add cache invalidation on data sync
- [ ] Test cache hit/miss performance

**Time Estimate**: 1 day

---

### Phase 2.C.5: API Pagination 📄

**Goal**: Add pagination to prevent large result sets

#### Tasks:
- [ ] Create `PaginatedResult<T>` DTO
- [ ] Add pagination to GET /api/github/commits endpoint
- [ ] Add pagination to GET /api/github/pull-requests endpoint
- [ ] Add pagination to Blazor components (MudTable)
- [ ] Add page size options (10, 25, 50, 100)
- [ ] Update UI to show total count and page info

**Time Estimate**: 0.5 days

---

### Phase 2.C.6: Code Cleanup 🧼

**Goal**: Improve code quality and maintainability

#### Tasks:
- [ ] Remove commented-out code
- [ ] Fix naming conventions (PascalCase, camelCase)
- [ ] Remove unused using statements
- [ ] Add XML documentation comments to public APIs
- [ ] Refactor long methods (>50 lines)
- [ ] Extract magic numbers to constants
- [ ] Run code analysis and fix warnings
- [ ] Format code with EditorConfig

**Time Estimate**: 0.5 days

---

### Phase 2.C.7: Logging Improvements 📝

**Goal**: Better structured logging across all services

#### Tasks:
- [ ] Replace string interpolation with structured logging
- [ ] Add correlation IDs for request tracking
- [ ] Log request/response times for API endpoints
- [ ] Add performance logging for slow queries (>1s)
- [ ] Configure log levels per environment (dev, prod)
- [ ] Add log scopes for context
- [ ] Review and reduce noisy logs

**Time Estimate**: 0.5 days

---

### Phase 2.C.8: Performance Testing 📊

**Goal**: Test with large repositories and concurrent users

#### Tasks:
- [ ] Create test repository with 1000+ commits
- [ ] Test sync performance with large repo
- [ ] Test dashboard load time with large dataset
- [ ] Simulate 10+ concurrent users
- [ ] Measure API response times (p50, p95, p99)
- [ ] Identify bottlenecks with profiling
- [ ] Document performance benchmarks
- [ ] Create performance regression tests

**Time Estimate**: 1 day

---

## 📈 Success Criteria

### Must Have ✅
- [ ] MetricsCalculationService has >80% test coverage
- [ ] All database indexes created and applied
- [ ] AsNoTracking used in all read queries
- [ ] Global exception handler in place
- [ ] Redis caching implemented for repos and metrics
- [ ] Pagination working on commits and PRs endpoints

### Nice to Have 🎯
- [ ] Code cleanup complete (no warnings)
- [ ] Structured logging improved
- [ ] Performance tests documented
- [ ] Load time <2s for dashboard with 100+ repos

---

## 🔄 Execution Order

**Week 1 (Days 1-3): Core Quality**
1. Phase 2.C.1: Testing (Day 1)
2. Phase 2.C.2: Database Performance (Day 2)
3. Phase 2.C.3: Error Handling (Day 3)

**Week 1 (Days 4-5): Performance & Polish**
4. Phase 2.C.4: Caching Layer (Day 4)
5. Phase 2.C.5: API Pagination (Day 4 afternoon)
6. Phase 2.C.6: Code Cleanup (Day 5 morning)
7. Phase 2.C.7: Logging Improvements (Day 5 afternoon)

**Week 2 (Day 6-7): Testing & Documentation**
8. Phase 2.C.8: Performance Testing (Day 6-7)
9. Update documentation with all changes

---

## 📝 Notes

- Each phase should have its own GitHub issue
- Each phase should have its own feature branch
- Each phase should have its own PR
- Keep sprint log updated with progress and learnings

---

**Last Updated**: November 13, 2025
**Status**: Ready to start
**Next Phase**: 2.C.1 - Testing & Quality
