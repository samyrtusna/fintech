namespace fintech.Domain.Entities
{
    public class UserCategorySetting
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? UserId { get; set; }
        public Guid CategoryId { get; set; }
        public bool IsEssential { get; set; } = false;
        public Category Category { get; set; } = null!;
    }
}
   