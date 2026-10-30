
using LibraryManagement.Data;
using LibraryManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Repositories;

public class AuditLogRepository
{
    private readonly AppDbContext _context;

    public AuditLogRepository(AppDbContext context)
    {
        _context = context;
    }

    // Add a new audit log
    public async Task AddAsync(AuditLog auditLog)
    {
        await _context.AuditLogs.AddAsync(auditLog);

        await _context.SaveChangesAsync();
    }

    // Get all audit logs
    public async Task<List<AuditLog>> GetAllAsync()
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .OrderByDescending(a => a.PerformedAt)
            .ToListAsync();
    }

    // Get audit logs by entity name
    public async Task<List<AuditLog>> GetByEntityAsync(
        string entityName)
    {
        return await _context.AuditLogs
            .AsNoTracking()
            .Where(a => a.EntityName == entityName)
            .OrderByDescending(a => a.PerformedAt)
            .ToListAsync();
    }
}
