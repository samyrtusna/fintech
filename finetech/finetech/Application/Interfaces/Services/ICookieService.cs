namespace fintech.Application.Interfaces.Services
{
    public interface ICookieService
    {
        void SetTokenToCookie(string token);
        void ClearTokenCookie();
        string RetrieveTokenFromCookie();
    }
}
