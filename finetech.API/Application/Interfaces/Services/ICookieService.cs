namespace fintech.API.Application.Interfaces.Services
{
    public interface ICookieService
    {
        void SetTokenToCookie(string token);
        void ClearCookieFromToken(); 
        string RetrieveTokenFromCookie();
    }
}
