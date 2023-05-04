namespace Brickalytics.Services
{    
    public interface IUserService : IDisposable    
    {    
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByCreatorNameAsync(string creatorName);
        Task<int> AddUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task UpdateUserPasswordAsync(User user);
        Task UpdateUserRefreshTokenAsync(User user);
    }    
}    