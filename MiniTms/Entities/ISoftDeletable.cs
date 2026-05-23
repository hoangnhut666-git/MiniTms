namespace MiniTms.Entities
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
        string? DeletedBy { get; set; }

        void MarkAsDeleted(string deletedBy);
        void Restore();
    }
}
