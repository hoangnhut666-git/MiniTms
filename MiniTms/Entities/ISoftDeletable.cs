using System;
using System.Collections.Generic;
using System.Text;

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
