
namespace LibraryManagement.Models
{
    public class AuditLog
    {
        public int AuditLogId { get; set; }

        public string Action { get; set; } = string.Empty;

        public string EntityName { get; set; } = string.Empty;

        public int? EntityId { get; set; }

        public string? Description { get; set; }

        public string? PerformedBy { get; set; }

        public DateTime PerformedAt { get; set; }
            = DateTime.UtcNow;
    }
}
