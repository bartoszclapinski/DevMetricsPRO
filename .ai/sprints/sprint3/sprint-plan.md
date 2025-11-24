# Sprint 3: Real-time Dashboard & Analytics 📊

**Duration**: 2 weeks (10 working days)  
**Goal**: Interactive charts, real-time updates with SignalR, and advanced analytics  
**Commitment**: 20-30 hours total (~2-3 hours/day)  

---

## 📋 Sprint Overview

This sprint focuses on creating a production-quality analytics dashboard with:
- **Week 1**: Interactive charts and visualizations
- **Week 2**: Real-time updates via SignalR, advanced metrics

Sprint 2 gave us the data pipeline. Sprint 3 brings it to life! 🎨

---

## 🎯 Sprint Goals

- [ ] Interactive charts displaying commit trends
- [ ] GitHub-style contribution heatmap
- [ ] Real-time dashboard updates via SignalR
- [ ] Advanced metrics (PR review time, code velocity)
- [ ] Time range selectors and filters
- [ ] Team leaderboards
- [ ] Mobile-responsive visualizations

---

# WEEK 1: Charts & Visualizations

## Phase 3.1: Setup Chart Library (Day 1)

**Goal**: Choose and configure charting library

**Decision**: Chart.js vs Plotly.NET

### Option A: Chart.js (Recommended for MVP)
**Pros**:
- Lightweight (bundle size)
- Excellent performance
- Great documentation
- Easy Blazor integration via JSInterop
- Professional look out of the box

**Cons**:
- Requires JSInterop
- Less advanced features than Plotly

### Option B: Plotly.NET
**Pros**:
- Native .NET
- Very powerful
- Scientific-grade charts
- No JSInterop needed

**Cons**:
- Larger bundle size
- More complex for simple charts
- Steeper learning curve

**Recommendation**: Start with **Chart.js** for simplicity, can add Plotly later if needed.

### Implementation Steps:

1. **Install Chart.js** (via CDN or npm):
   ```html
   <!-- In App.razor or index.html -->
   <script src="https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js"></script>
   ```

2. **Create JSInterop helper**:
   - Create `wwwroot/js/charts.js`
   - Wrapper functions for Chart.js initialization

3. **Create base chart component**:
   - `Components/Shared/ChartBase.razor`
   - Generic chart component with JSInterop

4. **Test with simple line chart**:
   - Display commit count over last 30 days
   - Verify JSInterop working

**Time Estimate**: 2-3 hours

---

## Phase 3.2: Commit Activity Chart (Day 2-3)

**Goal**: Line/area chart showing commit activity over time

### Step 3.2.1: Create Chart Data Service

- [ ] **Create `IChartDataService` interface**:
  ```csharp
  public interface IChartDataService
  {
      Task<CommitActivityChartDto> GetCommitActivityAsync(
          Guid? developerId,
          DateTime startDate,
          DateTime endDate,
          CancellationToken cancellationToken);
  }
  ```

- [ ] **Create `CommitActivityChartDto`**:
  ```csharp
  public record CommitActivityChartDto
  {
      public List<string> Labels { get; init; } = new();      // Dates
      public List<int> Values { get; init; } = new();          // Commit counts
      public int TotalCommits { get; init; }
      public double AveragePerDay { get; init; }
  }
  ```

- [ ] **Implement `ChartDataService`**:
  - Query commits grouped by date
  - Calculate aggregates
  - Format data for Chart.js

### Step 3.2.2: Create Line Chart Component

- [ ] **Create `Components/Charts/CommitActivityChart.razor`**:
  - Takes `CommitActivityChartDto` as parameter
  - Renders canvas element
  - Calls JSInterop to create Chart.js chart
  - Implements `IAsyncDisposable` to clean up

- [ ] **Add chart options**:
  - Time range selector (7d, 30d, 90d, 1y)
  - Toggle between line/area/bar
  - Smooth curves
  - Tooltips showing details

### Step 3.2.3: Integrate into Dashboard

- [ ] **Add to Home.razor**:
  - Place below metric cards
  - Add loading state
  - Error handling

**Time Estimate**: 4-5 hours

---

## Phase 3.3: Pull Request Statistics Chart (Day 3-4)

**Goal**: Bar chart showing PR statistics (open, merged, closed)

### Step 3.3.1: Create PR Chart Data

- [ ] **Add to `IChartDataService`**:
  ```csharp
  Task<PullRequestStatsChartDto> GetPullRequestStatsAsync(
      Guid? developerId,
      DateTime startDate,
      DateTime endDate,
      CancellationToken cancellationToken);
  ```

- [ ] **Create `PullRequestStatsChartDto`**:
  ```csharp
  public record PullRequestStatsChartDto
  {
      public int OpenCount { get; init; }
      public int MergedCount { get; init; }
      public int ClosedCount { get; init; }
      public double AverageMergeTime { get; init; } // Hours
      public double MergeRate { get; init; }        // Percentage
  }
  ```

### Step 3.3.2: Create Bar Chart Component

- [ ] **Create `Components/Charts/PullRequestStatsChart.razor`**:
  - Horizontal or vertical bars
  - Color coding (green=merged, blue=open, gray=closed)
  - Show percentages

### Step 3.3.3: Add PR Timeline Chart

- [ ] **Create timeline/gantt chart** showing:
  - PR open duration
  - Review time
  - Time to merge

**Time Estimate**: 3-4 hours

---

## Phase 3.4: Contribution Heatmap (Day 4-5)

**Goal**: GitHub-style contribution calendar

### Step 3.4.1: Create Heatmap Data Service

- [ ] **Add to `IChartDataService`**:
  ```csharp
  Task<ContributionHeatmapDto> GetContributionHeatmapAsync(
      Guid developerId,
      int numberOfWeeks,
      CancellationToken cancellationToken);
  ```

- [ ] **Create `ContributionHeatmapDto`**:
  ```csharp
  public record ContributionHeatmapDto
  {
      public List<DayContribution> Days { get; init; } = new();
      public int MaxContributions { get; init; }
      public int TotalContributions { get; init; }
  }
  
  public record DayContribution
  {
      public DateTime Date { get; init; }
      public int Count { get; init; }
      public string Level { get; init; } // "none", "low", "medium", "high"
  }
  ```

### Step 3.4.2: Create Heatmap Component

- [ ] **Create `Components/Charts/ContributionHeatmap.razor`**:
  - CSS Grid layout (7 columns for days of week)
  - Color intensity based on contribution count
  - Tooltips showing date and count
  - Could use CSS only (no Chart.js needed)

- [ ] **CSS for heatmap**:
  ```css
  .heatmap-cell {
      background-color: var(--contribution-level);
      border-radius: 2px;
      transition: transform 0.1s;
  }
  .heatmap-cell:hover {
      transform: scale(1.2);
  }
  ```

### Step 3.4.3: Add to Developer Profile Page

- [ ] Create `Components/Pages/DeveloperProfile.razor`
- [ ] Route: `/developers/{developerId}`
- [ ] Show developer details + heatmap

**Time Estimate**: 4-5 hours

---

## Phase 3.5: Team Leaderboard (Day 5)

**Goal**: Sortable table showing top contributors

### Step 3.5.1: Create Leaderboard Service

- [ ] **Create `ILeaderboardService`**:
  ```csharp
  Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(
      LeaderboardMetric metric,
      int topN,
      CancellationToken cancellationToken);
  ```

- [ ] **Create `LeaderboardEntryDto`**:
  ```csharp
  public record LeaderboardEntryDto
  {
      public int Rank { get; init; }
      public Guid DeveloperId { get; init; }
      public string DeveloperName { get; init; }
      public string? AvatarUrl { get; init; }
      public decimal Value { get; init; }
      public string Change { get; init; } // "+5%", "-2%"
  }
  ```

### Step 3.5.2: Create Leaderboard Component

- [ ] **Create `Components/Shared/Leaderboard.razor`**:
  - Sortable table
  - Trophy icons for top 3
  - Click to view developer profile
  - Show trend indicators (↑↓)

### Step 3.5.3: Add Multiple Leaderboards

- [ ] Commits leaderboard
- [ ] PRs leaderboard
- [ ] Code changes leaderboard
- [ ] Active days leaderboard

**Time Estimate**: 3-4 hours

---

# WEEK 2: Real-time Updates & Advanced Features

## Phase 3.6: SignalR Hub Setup (Day 6-7)

**Goal**: Real-time dashboard updates when data syncs

### Step 3.6.1: Create Metrics Hub

- [ ] **Create `Hubs/MetricsHub.cs`**:
  ```csharp
  public class MetricsHub : Hub
  {
      public async Task JoinDashboard(string userId)
      {
          await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
      }
      
      public async Task LeaveDashboard(string userId)
      {
          await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
      }
  }
  ```

- [ ] **Add SignalR endpoint** to `Program.cs`:
  ```csharp
  app.MapHub<MetricsHub>("/metricshub");
  ```

### Step 3.6.2: Create Hub Service

- [ ] **Create `IMetricsHubService`**:
  ```csharp
  public interface IMetricsHubService
  {
      Task NotifyMetricsUpdatedAsync(Guid userId, CancellationToken cancellationToken);
      Task NotifySyncCompletedAsync(Guid userId, SyncResultDto result, CancellationToken cancellationToken);
  }
  ```

- [ ] **Implement `MetricsHubService`**:
  - Uses `IHubContext<MetricsHub>`
  - Sends typed messages to clients
  - Handles connection failures

### Step 3.6.3: Update Background Job

- [ ] **Modify `SyncGitHubDataJob`**:
  - Inject `IMetricsHubService`
  - Call `NotifySyncCompletedAsync()` after sync
  - Include sync statistics in notification

**Time Estimate**: 4-5 hours

---

## Phase 3.7: Client-Side SignalR (Day 7-8)

**Goal**: Dashboard listens for real-time updates

### Step 3.7.1: Create SignalR Service

- [ ] **Create `Services/SignalRService.cs`**:
  ```csharp
  public class SignalRService : IAsyncDisposable
  {
      private HubConnection? _hubConnection;
      
      public async Task StartAsync()
      {
          _hubConnection = new HubConnectionBuilder()
              .WithUrl(NavigationManager.ToAbsoluteUri("/metricshub"))
              .WithAutomaticReconnect()
              .Build();
              
          _hubConnection.On<string>("MetricsUpdated", HandleMetricsUpdated);
          _hubConnection.On<SyncResultDto>("SyncCompleted", HandleSyncCompleted);
          
          await _hubConnection.StartAsync();
      }
  }
  ```

### Step 3.7.2: Update Dashboard Components

- [ ] **Update `Home.razor`**:
  - Inject `SignalRService`
  - Connect on initialization
  - Handle "MetricsUpdated" event → refresh data
  - Handle "SyncCompleted" event → show notification

- [ ] **Add visual feedback**:
  - Toast notification: "Data updated! Refreshing..."
  - Pulsing indicator during sync
  - Smooth transition animations

### Step 3.7.3: Connection Resilience

- [ ] Handle disconnections gracefully
- [ ] Auto-reconnect with exponential backoff
- [ ] Show connection status indicator
- [ ] Queue updates during disconnection

**Time Estimate**: 4-5 hours

---

## Phase 3.8: Advanced Metrics (Day 8-9)

**Goal**: Calculate more sophisticated metrics

### Step 3.8.1: PR Review Time Metrics

- [ ] **Add to `IMetricsCalculationService`**:
  ```csharp
  Task<ReviewTimeMetricsDto> CalculateReviewTimeMetricsAsync(
      Guid developerId,
      DateTime startDate,
      DateTime endDate,
      CancellationToken cancellationToken);
  ```

- [ ] **Calculate**:
  - Average time from PR open to first review
  - Average time from PR open to merge
  - Review response rate
  - PR complexity (files changed, lines)

### Step 3.8.2: Code Velocity Metrics

- [ ] **Calculate velocity trends**:
  - Commits per week (moving average)
  - Lines of code per week
  - PR merge rate over time
  - Identify productivity patterns

### Step 3.8.3: Team Collaboration Metrics

- [ ] **Calculate team metrics**:
  - Cross-repository contributions
  - Code review participation
  - Most collaborated repositories
  - Knowledge distribution (bus factor)

**Time Estimate**: 5-6 hours

---

## Phase 3.9: Time Range Filters (Day 9)

**Goal**: Filter all charts by custom date ranges

### Step 3.9.1: Create Time Range Component

- [ ] **Create `Components/Shared/TimeRangeSelector.razor`**:
  - Preset buttons (7d, 30d, 90d, 1y, All)
  - Custom date picker
  - Emits `EventCallback` on change

### Step 3.9.2: Add State Management

- [ ] **Create `Services/DashboardStateService.cs`**:
  ```csharp
  public class DashboardStateService
  {
      public DateTime StartDate { get; set; }
      public DateTime EndDate { get; set; }
      public event Action? OnStateChanged;
      
      public void UpdateDateRange(DateTime start, DateTime end)
      {
          StartDate = start;
          EndDate = end;
          OnStateChanged?.Invoke();
      }
  }
  ```

- [ ] Register as **Scoped** service

### Step 3.9.3: Update All Charts

- [ ] Subscribe to state changes
- [ ] Reload data when date range changes
- [ ] Show loading state during refresh

**Time Estimate**: 3-4 hours

---

## Phase 3.10: Polish & Performance (Day 10)

**Goal**: Final touches and optimization

### Step 3.10.1: Loading States

- [ ] **Skeleton loaders** for charts
- [ ] Shimmer effect during load
- [ ] Progressive loading (metrics → charts)

### Step 3.10.2: Error States

- [ ] Empty state when no data
- [ ] Error state with retry button
- [ ] Friendly error messages

### Step 3.10.3: Performance Optimization

- [ ] Virtualize large lists
- [ ] Debounce SignalR events
- [ ] Memoize expensive calculations
- [ ] Lazy load charts (only visible ones)

### Step 3.10.4: Responsive Design

- [ ] Test on mobile (charts stack vertically)
- [ ] Touch-friendly controls
- [ ] Readable text sizes

### Step 3.10.5: Accessibility

- [ ] ARIA labels on charts
- [ ] Keyboard navigation
- [ ] Screen reader support
- [ ] High contrast mode

**Time Estimate**: 4-5 hours

---

## ✅ Sprint 3 Success Criteria

### Functional Requirements
- [ ] Line chart showing commit activity over time
- [ ] Bar chart showing PR statistics
- [ ] GitHub-style contribution heatmap
- [ ] Team leaderboard (sortable, top 10)
- [ ] Real-time updates via SignalR (no page refresh needed)
- [ ] Time range filters working on all charts
- [ ] Advanced metrics calculated (review time, velocity)

### Non-Functional Requirements
- [ ] Charts render in <500ms
- [ ] SignalR connection stable (auto-reconnect)
- [ ] Mobile responsive (all charts visible)
- [ ] Loading states for all async operations
- [ ] Error handling comprehensive
- [ ] 0 console errors
- [ ] Accessible (WCAG AA minimum)

### Quality Gates
- [ ] All charts tested with real data
- [ ] SignalR tested with multiple clients
- [ ] Performance tested (100+ data points)
- [ ] Mobile tested on real device
- [ ] No memory leaks (dispose properly)

---

## 🎯 Definition of Done

A feature is done when:
1. ✅ Code implemented and working
2. ✅ Unit tests written (where applicable)
3. ✅ Integrated into dashboard
4. ✅ Tested manually
5. ✅ Responsive design verified
6. ✅ Error states handled
7. ✅ Sprint log updated
8. ✅ No linter warnings

---

## 📊 Estimated Effort

| Phase | Hours | Complexity |
|-------|-------|------------|
| 3.1: Chart Setup | 2-3h | Low |
| 3.2: Commit Activity Chart | 4-5h | Medium |
| 3.3: PR Statistics Chart | 3-4h | Medium |
| 3.4: Contribution Heatmap | 4-5h | Medium |
| 3.5: Team Leaderboard | 3-4h | Low |
| 3.6: SignalR Hub Setup | 4-5h | Medium |
| 3.7: Client SignalR | 4-5h | Medium |
| 3.8: Advanced Metrics | 5-6h | High |
| 3.9: Time Range Filters | 3-4h | Medium |
| 3.10: Polish & Performance | 4-5h | Low |
| **Total** | **36-46h** | **~4 weeks at 10h/week** |

---

## 🚧 Risks & Mitigation

| Risk | Impact | Mitigation |
|------|--------|------------|
| Chart.js learning curve | Medium | Start with simple charts, iterate |
| SignalR scaling issues | High | Test with multiple clients early |
| Performance with large datasets | High | Implement pagination, virtualization |
| Browser compatibility | Medium | Test on Chrome, Firefox, Edge |
| Mobile performance | Medium | Lazy load charts, optimize bundles |

---

## 📚 Resources

### Chart.js
- [Chart.js Documentation](https://www.chartjs.org/docs/latest/)
- [Chart.js Samples](https://www.chartjs.org/samples/latest/)
- [Blazor + Chart.js Tutorial](https://chrissainty.com/working-with-javascript-interop-in-blazor/)

### SignalR
- [SignalR Documentation](https://docs.microsoft.com/en-us/aspnet/core/signalr)
- [SignalR Hubs](https://docs.microsoft.com/en-us/aspnet/core/signalr/hubs)
- [SignalR Client](https://docs.microsoft.com/en-us/aspnet/core/signalr/javascript-client)

### Design Inspiration
- [GitHub Contributions Graph](https://github.com/)
- [Grafana Dashboards](https://grafana.com/)
- [Datadog Dashboards](https://www.datadoghq.com/)

---

## 🎓 Learning Objectives

By the end of Sprint 3, you'll understand:
- ✅ Chart.js integration with Blazor
- ✅ JavaScript Interop patterns
- ✅ SignalR real-time communication
- ✅ Hub-based architecture
- ✅ Client-side state management
- ✅ Data visualization best practices
- ✅ Performance optimization techniques
- ✅ Responsive chart design

---

**Sprint 3 Status**: 🚀 **READY TO START**  
**First Phase**: Phase 3.1 - Chart Library Setup  
**Estimated Completion**: 2-4 weeks (depending on hours/week)

Let's build an amazing dashboard! 📊✨

