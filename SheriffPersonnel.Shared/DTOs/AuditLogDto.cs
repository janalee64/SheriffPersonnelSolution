using System;

namespace SheriffPersonnel.Shared.DTOs
{
    public class AuditLogDto
    {
        public long AuditLogID { get; set; }
        public string SchemaName { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string PrimaryKeyValues { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string ChangedBy { get; set; } = string.Empty;
        public DateTime ChangedDate { get; set; }
        public string OldValuesJSON { get; set; } = string.Empty;
        public string? NewValuesJSON { get; set; }
    }
}

