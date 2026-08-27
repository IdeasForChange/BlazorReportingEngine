using Microsoft.EntityFrameworkCore;
using Smbc.Risk.ReportingEngine.Domain.Entities;
using Smbc.Risk.ReportingEngine.Domain.Repositories;
using Smbc.Risk.ReportingEngine.Domain.Shared.Enums;

namespace Smbc.Risk.ReportingEngine.Infrastructure.Data.EntityFramework.Repositories;

public class ReportRunnerQueueRepository(ApplicationDbContext dbContext)
    : BaseRepository<ReportRunnerQueue>(dbContext), IReportRunnerQueueRepository
{
    public async Task<ReportRunnerQueue?> GetNextPendingItemAsync()
    {
        // Pessimistic Locking with EF Core Execution via raw SQL or transaction block
        var pendingItem = await _dbContext.ReportRunnerQueues
            .Where(q => q.Status == QueueStatus.Pending && q.IsActive)
            .OrderBy(q => q.CreatedAtUtc)
            .FirstOrDefaultAsync();

        if (pendingItem != null)
        {
            pendingItem.Status = QueueStatus.Processing;
            pendingItem.StartedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }

        return pendingItem;
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

    public async Task UpdateQueueStatusAsync(long queueItemId, QueueStatus status, string? errorMessage = null)
    {
        var item = await _dbContext.ReportRunnerQueues.FindAsync(queueItemId);
        if (item != null)
        {
            item.Status = status;
            item.ErrorMessage = errorMessage;
            item.UpdatedAtUtc = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
        }
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
