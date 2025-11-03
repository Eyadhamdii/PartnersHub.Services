using PartnersHub.CiamMigrationTool.Models;

namespace PartnersHub.CiamMigrationTool.Services;

public interface IMigrationRepository
{
    Task<MigrationRecord?> GetByMicrosoftUserIdAsync(string microsoftUserId);
    Task<List<MigrationRecord>> GetAllAsync();
    Task<List<MigrationRecord>> GetFailedMigrationsAsync(int maxRetries);
    Task AddAsync(MigrationRecord record);
    Task UpdateAsync(MigrationRecord record);
    Task<List<MigrationRecord>> GetByStatusAsync(MigrationStatus status);
    Task<bool> EnsureDatabaseCreatedAsync();
}

// In-memory repository for fallback/testing
public class InMemoryMigrationRepository : IMigrationRepository
{
    private static readonly List<MigrationRecord> _records = new();
    private static int _nextId = 1;
    private readonly ISimpleLogger _logger;

    public InMemoryMigrationRepository(ISimpleLogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> EnsureDatabaseCreatedAsync()
    {
        await Task.CompletedTask;
        _logger.LogInformation("In-memory repository - no database creation needed");
        return true;
    }

    public async Task<MigrationRecord?> GetByMicrosoftUserIdAsync(string microsoftUserId)
    {
        await Task.Delay(10); // Simulate async operation
        return _records.FirstOrDefault(r => r.MicrosoftUserId == microsoftUserId);
    }

    public async Task<List<MigrationRecord>> GetAllAsync()
    {
        await Task.Delay(10); // Simulate async operation
        return _records.OrderByDescending(r => r.CreatedAt).ToList();
    }

    public async Task<List<MigrationRecord>> GetFailedMigrationsAsync(int maxRetries)
    {
        await Task.Delay(10); // Simulate async operation
        return _records
            .Where(r => r.Status == MigrationStatus.Failed && r.RetryCount < maxRetries)
            .ToList();
    }

    public async Task AddAsync(MigrationRecord record)
    {
        await Task.Delay(10); // Simulate async operation
        
        if (record.Id == 0)
        {
            record.Id = _nextId++;
        }
        
        _records.Add(record);
        _logger.LogInformation($"Added migration record for user {record.MicrosoftUserPrincipalName} (in-memory)");
    }

    public async Task UpdateAsync(MigrationRecord record)
    {
        await Task.Delay(10); // Simulate async operation
        
        var existingRecord = _records.FirstOrDefault(r => r.Id == record.Id);
        if (existingRecord != null)
        {
            existingRecord.Status = record.Status;
            existingRecord.CiamUserId = record.CiamUserId;
            existingRecord.CiamUserName = record.CiamUserName;
            existingRecord.ErrorMessage = record.ErrorMessage;
            existingRecord.MigratedAt = record.MigratedAt;
            existingRecord.RetryCount = record.RetryCount;
            
            _logger.LogInformation($"Updated migration record for user {record.MicrosoftUserPrincipalName} - Status: {record.Status} (in-memory)");
        }
    }

    public async Task<List<MigrationRecord>> GetByStatusAsync(MigrationStatus status)
    {
        await Task.Delay(10); // Simulate async operation
        return _records.Where(r => r.Status == status).ToList();
    }
}