namespace fintech.API.Domain.Entities
{
    public class Options
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
