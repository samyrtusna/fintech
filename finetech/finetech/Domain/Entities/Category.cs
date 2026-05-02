using fintech.Domain.Enums;

namespace fintech.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; } 
        public string Name { get; set; } = null!;
        public CategoryType Type { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public Category? ParentCategory { get; set; }
        public bool IsSystem { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false; 
        public List<FinancialTransaction> Transactions { get; set; } = [];
    } 
}
