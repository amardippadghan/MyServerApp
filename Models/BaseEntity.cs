namespace MyServerApp.Models
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public int Status { get; set; } = 1; // 1 = Active, 0 = Soft Deleted
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public bool IsActive => Status == 1;
        public bool IsDeleted => Status == 0;
    }
}
