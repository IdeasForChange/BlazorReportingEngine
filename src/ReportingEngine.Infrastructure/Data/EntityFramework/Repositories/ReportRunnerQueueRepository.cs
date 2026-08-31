using Microsoft.EntityFrameworkCore;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;

public class ReportRunnerQueueRepository(ApplicationDbContext dbContext)
    : BaseRepository<ReportRunnerQueue>(dbContext), IReportRunnerQueueRepository
{
    public async Task<ReportRunnerQueue?> GetNextPendingJobAsync(CancellationToken cancellationToken = default)
    {
        // Pessimistic Locking with EF Core Execution via raw SQL or transaction block
        var pendingItem = await _dbContext.ReportRunnerQueues
            .Where(q => q.Status == QueueStatus.Pending && q.IsActive)
            .OrderBy(q => q.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);

        if (pendingItem != null)
        {
            pendingItem.Status = QueueStatus.Processing;
            pendingItem.StartedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        return pendingItem;
    }

    public async Task<List<long>> ClaimPendingJobIdsAsync(int batchSize, CancellationToken cancellationToken)
    {
        // Fetch pending jobs ordering by creation time
        var pendingJobs = await _dbContext.ReportRunnerQueues
            .Where(q => q.Status == QueueStatus.Pending && q.IsActive)
            .OrderBy(q => q.CreatedAtUtc)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        var claimedIds = new List<long>();

        foreach (var job in pendingJobs)
        {
            // Mark as Processing immediately so another thread or app instance doesn't pick it up
            job.Status = QueueStatus.Processing;
            job.StartedAtUtc = DateTime.UtcNow;
            job.UpdatedAtUtc = DateTime.UtcNow;
            job.UpdatedBy = "JobProcessor-Claimed";
            claimedIds.Add(job.Id);
        }

        if (claimedIds.Any())
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return claimedIds;
    }

    public async Task<IEnumerable<ReportRunnerQueue>> GetQueueByFilterAsync(string filter)
    {
        DateTime startDate = filter switch
        {
            "Yesterday" => DateTime.UtcNow.Date.AddDays(-1),
            "Week" => DateTime.UtcNow.Date.AddDays(-7),
            "Month" => DateTime.UtcNow.Date.AddMonths(-1),
            _ => DateTime.UtcNow.Date
        };

        DateTime endDate = filter == "Yesterday" ? DateTime.UtcNow.Date : DateTime.UtcNow.AddDays(1);

        return await _dbContext.ReportRunnerQueues
            .Include(q => q.ReportMaster)
            .AsNoTracking()
            .Where(q => q.CreatedAtUtc >= startDate && q.CreatedAtUtc < endDate)
            .OrderByDescending(q => q.CreatedAtUtc)
            .ToListAsync();
    }

    public async Task UpdateJobStatusAsync(long jobId, QueueStatus status, int progress, string? outputFilePath = null, string? errorMessage = null, CancellationToken cancellationToken = default)
    {
        var job = await _dbContext.ReportRunnerQueues.FindAsync(new object[] { jobId }, cancellationToken);
        if (job == null) return;

        job.Status = status;
        job.ProgressPercentage = progress;
        job.UpdatedBy = "JobProcessor";
        job.UpdatedAtUtc = DateTime.UtcNow;

        if (status == QueueStatus.Processing && job.StartedAtUtc == null) // Processing
        {
            job.StartedAtUtc = DateTime.UtcNow;
        }
        if (status == QueueStatus.Completed || status == QueueStatus.Failed) // Completed or Failed
        {
            job.CompletedAtUtc = DateTime.UtcNow;
        }

        if (outputFilePath != null) job.OutputFilePath = outputFilePath;
        if (errorMessage != null) job.ErrorMessage = errorMessage;

        await _dbContext.SaveChangesAsync(cancellationToken);
    }


    public async Task CompleteQueueItemAsync(long queueItemId, string outputFilePath)
    {
        var item = await _dbContext.ReportRunnerQueues.FindAsync(queueItemId);
        if (item != null)
        {
            item.Status = QueueStatus.Completed;
            item.OutputFilePath = outputFilePath;
            item.CompletedAtUtc = DateTime.UtcNow;
            item.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
    }
}
