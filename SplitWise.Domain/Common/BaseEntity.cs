namespace SplitWise.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; } 
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; } = true;  
    }
}
