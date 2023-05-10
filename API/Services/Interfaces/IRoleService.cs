namespace Brickalytics.Services
{    
    public interface IRoleService : IDisposable    
    {    
        Task<List<Role>> GetRolesAsync();
    }    
}    