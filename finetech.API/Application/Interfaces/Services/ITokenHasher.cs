namespace fintech.API.Application.Interfaces.Services
{
    public interface ITokenHasher 
    {
        string HashToken(string token);
    }
}
