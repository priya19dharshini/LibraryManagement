
using LibraryManagement.Models;
using LibraryManagement.Repositories;

namespace LibraryManagement.Services;

public class AuditLogService
{
    private readonly AuditLogRepository _repository;

    public AuditLogService(AuditLogRepository repository)
    {
        _repository = repository;
    }

    // Record an activity
    public async Task LogAsync(
        string action,
        string entityName,
        int? entityId,
        string description,
        string? performedBy = "System")
    {
        var auditLog = new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            Description = description,
            PerformedBy = performedBy,
            PerformedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(auditLog);
    }

    // View all audit logs
    public async Task<List<AuditLog>> GetAllLogsAsync()
    {
        return await _repository.GetAllAsync();
    }

    // View logs by entity
    public async Task<List<AuditLog>> GetLogsByEntityAsync(
        string entityName)
    {
        return await _repository.GetByEntityAsync(entityName);
    }
}
