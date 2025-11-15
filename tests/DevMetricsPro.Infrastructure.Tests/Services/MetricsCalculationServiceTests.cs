using System.Linq.Expressions;
using DevMetricsPro.Core.Entities;
using DevMetricsPro.Core.Enums;
using DevMetricsPro.Core.Interfaces;
using DevMetricsPro.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DevMetricsPro.Infrastructure.Tests.Services;

public class MetricsCalculationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<MetricsCalculationService>> _loggerMock;
    private readonly MetricsCalculationService _service;

    public MetricsCalculationServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<MetricsCalculationService>>();
        _service = new MetricsCalculationService(_unitOfWorkMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CalculateMetricsForDeveloperAsync_WithCommitsAndPRs_CalculatesAllMetrics()
    {
        // Arrange
        var developerId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var repositoryId = Guid.NewGuid();

        var commits = new List<Commit>
        {
            new Commit
            {
                Id = Guid.NewGuid(),
                DeveloperId = developerId,
                RepositoryId = repositoryId,
                Sha = "abc123",
                Message = "Test commit 1",
                LinesAdded = 100,
                LinesRemoved = 50,
                CommittedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Commit
            {
                Id = Guid.NewGuid(),
                DeveloperId = developerId,
                RepositoryId = repositoryId,
                Sha = "def456",
                Message = "Test commit 2",
                LinesAdded = 200,
                LinesRemoved = 30,
                CommittedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        var pullRequests = new List<PullRequest>
        {
            new PullRequest
            {
                Id = Guid.NewGuid(),
                AuthorId = developerId,
                RepositoryId = repositoryId,
                ExternalId = "1",
                Title = "Test PR 1",
                Status = PullRequestStatus.Merged,
                CreatedAt = DateTime.UtcNow.AddDays(-7)
            }
        };

        var commitRepoMock = new Mock<IRepository<Commit>>();
        commitRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Commit, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Commit, bool>> predicate, CancellationToken _) =>
                commits.Where(predicate.Compile()).ToList());

        var prRepoMock = new Mock<IRepository<PullRequest>>();
        prRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<PullRequest, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<PullRequest, bool>> predicate, CancellationToken _) =>
                pullRequests.Where(predicate.Compile()).ToList());

        var metricRepoMock = new Mock<IRepository<Metric>>();
        var metricsStore = new List<Metric>();
        metricRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Metric, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Metric, bool>> predicate, CancellationToken _) =>
                metricsStore.Where(predicate.Compile()).ToList());

        _unitOfWorkMock.Setup(u => u.Repository<Commit>()).Returns(commitRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<PullRequest>()).Returns(prRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<Metric>()).Returns(metricRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        await _service.CalculateMetricsForDeveloperAsync(developerId, startDate, endDate);

        // Assert
        metricRepoMock.Verify(r => r.AddAsync(
            It.Is<Metric>(m => m.DeveloperId == developerId && m.Type == MetricType.Commits && m.Value == 2),
            It.IsAny<CancellationToken>()), Times.Once);

        metricRepoMock.Verify(r => r.AddAsync(
            It.Is<Metric>(m => m.DeveloperId == developerId && m.Type == MetricType.LinesAdded && m.Value == 300),
            It.IsAny<CancellationToken>()), Times.Once);

        metricRepoMock.Verify(r => r.AddAsync(
            It.Is<Metric>(m => m.DeveloperId == developerId && m.Type == MetricType.PullRequests && m.Value == 1),
            It.IsAny<CancellationToken>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CalculateMetricsForDeveloperAsync_WithNoCommits_CalculatesZeroMetrics()
    {
        // Arrange
        var developerId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;

        var emptyCommits = new List<Commit>();
        var emptyPrs = new List<PullRequest>();

        var commitRepoMock = new Mock<IRepository<Commit>>();
        commitRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Commit, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Commit, bool>> predicate, CancellationToken _) =>
                emptyCommits.Where(predicate.Compile()).ToList());

        var prRepoMock = new Mock<IRepository<PullRequest>>();
        prRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<PullRequest, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<PullRequest, bool>> predicate, CancellationToken _) =>
                emptyPrs.Where(predicate.Compile()).ToList());

        var metricRepoMock = new Mock<IRepository<Metric>>();
        metricRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Metric, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Metric>());

        _unitOfWorkMock.Setup(u => u.Repository<Commit>()).Returns(commitRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<PullRequest>()).Returns(prRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<Metric>()).Returns(metricRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        await _service.CalculateMetricsForDeveloperAsync(developerId, startDate, endDate);

        // Assert
        metricRepoMock.Verify(r => r.AddAsync(
            It.Is<Metric>(m => m.DeveloperId == developerId && m.Type == MetricType.Commits && m.Value == 0),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CalculateMetricsForAllDevelopersAsync_WithMultipleDevelopers_ProcessesAll()
    {
        // Arrange
        var developer1Id = Guid.NewGuid();
        var developer2Id = Guid.NewGuid();

        var developers = new List<Developer>
        {
            new Developer { Id = developer1Id, Email = "dev1@test.com" },
            new Developer { Id = developer2Id, Email = "dev2@test.com" }
        };

        var developerRepoMock = new Mock<IRepository<Developer>>();
        developerRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(developers);

        var commitRepoMock = new Mock<IRepository<Commit>>();
        commitRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Commit, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Commit>());

        var prRepoMock = new Mock<IRepository<PullRequest>>();
        prRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<PullRequest, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PullRequest>());

        var metricRepoMock = new Mock<IRepository<Metric>>();
        metricRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Metric, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Metric>());

        _unitOfWorkMock.Setup(u => u.Repository<Developer>()).Returns(developerRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<Commit>()).Returns(commitRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<PullRequest>()).Returns(prRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<Metric>()).Returns(metricRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        // Act
        await _service.CalculateMetricsForAllDevelopersAsync();

        // Assert
        metricRepoMock.Verify(r => r.AddAsync(
            It.Is<Metric>(m => m.DeveloperId == developer1Id),
            It.IsAny<CancellationToken>()), Times.Exactly(5));

        metricRepoMock.Verify(r => r.AddAsync(
            It.Is<Metric>(m => m.DeveloperId == developer2Id),
            It.IsAny<CancellationToken>()), Times.Exactly(5));
    }

    [Fact]
    public async Task CalculateMetricsForDeveloperAsync_WithDateRangeFilter_FiltersCorrectly()
    {
        // Arrange
        var developerId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-10);
        var endDate = DateTime.UtcNow.AddDays(-5);
        var repositoryId = Guid.NewGuid();

        var commits = new List<Commit>
        {
            new Commit
            {
                Id = Guid.NewGuid(),
                DeveloperId = developerId,
                RepositoryId = repositoryId,
                Sha = "in-range",
                LinesAdded = 100,
                LinesRemoved = 50,
                CommittedAt = DateTime.UtcNow.AddDays(-7) // Within range
            },
            new Commit
            {
                Id = Guid.NewGuid(),
                DeveloperId = developerId,
                RepositoryId = repositoryId,
                Sha = "out-of-range",
                LinesAdded = 500,
                LinesRemoved = 100,
                CommittedAt = DateTime.UtcNow.AddDays(-15) // Outside range
            }
        };

        var commitRepoMock = new Mock<IRepository<Commit>>();
        commitRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Commit, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Commit, bool>> predicate, CancellationToken _) =>
                commits.Where(predicate.Compile()).ToList());

        var prRepoMock = new Mock<IRepository<PullRequest>>();
        var emptyPrList = new List<PullRequest>();
        prRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<PullRequest, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<PullRequest, bool>> predicate, CancellationToken _) =>
                emptyPrList.Where(predicate.Compile()).ToList());

        var metricRepoMock = new Mock<IRepository<Metric>>();
        var metricsStore = new List<Metric>();
        metricRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Metric, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Metric, bool>> predicate, CancellationToken _) =>
                metricsStore.Where(predicate.Compile()).ToList());

        _unitOfWorkMock.Setup(u => u.Repository<Commit>()).Returns(commitRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<PullRequest>()).Returns(prRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<Metric>()).Returns(metricRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        await _service.CalculateMetricsForDeveloperAsync(developerId, startDate, endDate);

        // Assert - Should only count commit within date range
        metricRepoMock.Verify(r => r.AddAsync(
            It.Is<Metric>(m => m.DeveloperId == developerId && m.Type == MetricType.Commits && m.Value == 1),
            It.IsAny<CancellationToken>()), Times.Once);

        metricRepoMock.Verify(r => r.AddAsync(
            It.Is<Metric>(m => m.DeveloperId == developerId && m.Type == MetricType.LinesAdded && m.Value == 100),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CalculateMetricsForDeveloperAsync_WithExistingMetric_UpdatesInsteadOfCreating()
    {
        // Arrange
        var developerId = Guid.NewGuid();
        var startDate = DateTime.UtcNow.AddDays(-30);
        var endDate = DateTime.UtcNow;
        var repositoryId = Guid.NewGuid();

        var commits = new List<Commit>
        {
            new Commit
            {
                Id = Guid.NewGuid(),
                DeveloperId = developerId,
                RepositoryId = repositoryId,
                Sha = "abc123",
                LinesAdded = 100,
                LinesRemoved = 50,
                CommittedAt = DateTime.UtcNow.AddDays(-5)
            }
        };

        var existingMetric = new Metric
        {
            Id = Guid.NewGuid(),
            DeveloperId = developerId,
            Type = MetricType.Commits,
            Value = 5, // Old value
            Timestamp = DateTime.UtcNow.AddDays(-60),
            CreatedAt = DateTime.UtcNow.AddDays(-60)
        };

        var commitRepoMock = new Mock<IRepository<Commit>>();
        commitRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Commit, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Commit, bool>> predicate, CancellationToken _) =>
                commits.Where(predicate.Compile()).ToList());

        var prRepoMock = new Mock<IRepository<PullRequest>>();
        var emptyPrsForExisting = new List<PullRequest>();
        prRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<PullRequest, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<PullRequest, bool>> predicate, CancellationToken _) =>
                emptyPrsForExisting.Where(predicate.Compile()).ToList());

        var metricRepoMock = new Mock<IRepository<Metric>>();
        var metricStore = new List<Metric> { existingMetric };
        metricRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Metric, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Metric, bool>> predicate, CancellationToken _) =>
                metricStore.Where(predicate.Compile()).ToList());

        _unitOfWorkMock.Setup(u => u.Repository<Commit>()).Returns(commitRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<PullRequest>()).Returns(prRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<Metric>()).Returns(metricRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        await _service.CalculateMetricsForDeveloperAsync(developerId, startDate, endDate);

        // Assert - Should update existing metric, not create new one
        metricRepoMock.Verify(r => r.UpdateAsync(
            It.Is<Metric>(m => m.Id == existingMetric.Id && m.Value == 1),
            It.IsAny<CancellationToken>()), Times.Once);

        metricRepoMock.Verify(r => r.AddAsync(
            It.Is<Metric>(m => m.Type == MetricType.Commits),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CalculateMetricsForAllDevelopersAsync_WithErrorOnOneDeveloper_ContinuesProcessing()
    {
        // Arrange
        var developer1Id = Guid.NewGuid();
        var developer2Id = Guid.NewGuid();

        var developers = new List<Developer>
        {
            new Developer { Id = developer1Id, Email = "dev1@test.com" },
            new Developer { Id = developer2Id, Email = "dev2@test.com" }
        };

        var developerRepoMock = new Mock<IRepository<Developer>>();
        developerRepoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(developers);

        var commitRepoMock = new Mock<IRepository<Commit>>();
        // First developer: throw error, second: return empty list
        commitRepoMock.SetupSequence(r => r.FindAsync(It.IsAny<Expression<Func<Commit, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"))
            .ReturnsAsync(new List<Commit>());

        var prRepoMock = new Mock<IRepository<PullRequest>>();
        prRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<PullRequest, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PullRequest>());

        var metricRepoMock = new Mock<IRepository<Metric>>();
        metricRepoMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Metric, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Metric>());

        _unitOfWorkMock.Setup(u => u.Repository<Developer>()).Returns(developerRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<Commit>()).Returns(commitRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<PullRequest>()).Returns(prRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Repository<Metric>()).Returns(metricRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        // Act
        await _service.CalculateMetricsForAllDevelopersAsync();

        // Assert - Should still process developer2 despite error on developer1
        metricRepoMock.Verify(r => r.AddAsync(
            It.Is<Metric>(m => m.DeveloperId == developer2Id),
            It.IsAny<CancellationToken>()), Times.Exactly(5));
    }
}

