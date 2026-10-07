namespace fintech.API.Domain.Entities
{
    public class Option
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
