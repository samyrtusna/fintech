namespace fintech.Application.Interfaces.Services
{
    public interface ITokenHasher 
    {
        string HashToken(string token);
    }
}
