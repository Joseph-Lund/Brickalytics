namespace Brickalytics.Helpers
{
    public interface ITokenHelper: IDisposable
    {
        bool IsUserAdmin(string token);
        int GetUserId(string token);
    }    
}    