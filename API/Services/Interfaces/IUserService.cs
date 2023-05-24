using Brickalytics.Models;

namespace Brickalytics.Services
{    
    public interface IUserService : IDisposable    
    {    
        Task<List<User>> GetUsersAsync();
        Task<User> GetUserByIdAsync(int id);
        Task<List<Payment>> GetUserPaymentsAsync(int userId);
        Task<List<UserRate>> GetUserRatesAsync(User user);
        Task<User> GetUserByCreatorNameAsync(string creatorName);
        Task<int> AddUserAsync(User user);
        Task<int> AddUserPaymentAsync(Payment payment);
        Task UpdateUserAsync(User user);
        Task UpdateUserPasswordAsync(User user);
        Task UpdateUserRefreshTokenAsync(User user);
        Task AddUpdateUserRateAsync(UserRate userRate);
        Task DeleteUserRateAsync(UserRate userRate);
    }    
}    