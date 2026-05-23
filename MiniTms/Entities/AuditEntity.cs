using MiniTms.Shared.Helpers;

namespace MiniTms.Entities
{
    public abstract class AuditEntity : IAuditableEntity, ISoftDeletable
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Soft delete properties
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Audit methods
        public void MarkAsCreated(string createdBy)
        {
            CreatedAt = DateTimeHelper.GetVietnamTime();
            CreatedBy = createdBy;
            UpdatedAt = null;
            UpdatedBy = null;
        }

        public void MarkAsUpdated(string updatedBy)
        {
            UpdatedAt = DateTimeHelper.GetVietnamTime();
            UpdatedBy = updatedBy;
        }

        public void MarkAsDeleted(string deletedBy)
        {
            IsDeleted = true;
            DeletedAt = DateTimeHelper.GetVietnamTime();
            DeletedBy = deletedBy;
        }

        public void Restore()
        {
            IsDeleted = false;
            DeletedAt = null;
            DeletedBy = null;
        }
    }
}
