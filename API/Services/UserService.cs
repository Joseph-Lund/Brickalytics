using Dapper;
using System.Data;
using Brickalytics.Models;

namespace Brickalytics.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IDapperService _dapper;

        public UserService(ILogger<UserService> logger, IDapperService dapper)
        {
            _logger = logger;
            _dapper = dapper;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var result = await Task.FromResult(_dapper.GetAll<User>("GetUsers"));
            return result;
        }
        public async Task<User> GetUserByIdAsync(int id)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", id);

            var result = await Task.FromResult(_dapper.Get<User>("GetUserById", parameters));
            return result;
        }
        public async Task<User> GetUserByCreatorNameAsync(string creatorName)
        {
            var parameters = new DynamicParameters();
            parameters.Add("CreatorName", creatorName);

            var result = await Task.FromResult(_dapper.Get<User>("GetUserByCreatorName", parameters));
            return result;
        }
        public async Task<int> AddUserAsync(User user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", DbType.Int32, direction:ParameterDirection.Output);
            parameters.Add("CreatorName", user.CreatorName);
            parameters.Add("Email", user.Email);
            parameters.Add("Active", user.Active);
            parameters.Add("Hash", user.Hash);
            parameters.Add("Salt", user.Salt);
            parameters.Add("RoleId", user.RoleId);

            await Task.FromResult(_dapper.Insert<User>("AddUser", parameters));

            var id = parameters.Get<int>("Id"); 
            return id;
        }
        public async Task UpdateUserAsync(User user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", user.Id);
            parameters.Add("CreatorName", user.CreatorName);
            parameters.Add("CollectionId", user.CollectionId);
            parameters.Add("Email", user.Email);
            parameters.Add("Active", user.Active);
            parameters.Add("RoleId", user.RoleId);

            await Task.FromResult(_dapper.Update<User>("UpdateUser", parameters));
        }
        public async Task UpdateUserPasswordAsync(User user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", user.Id);
            parameters.Add("Hash", user.Hash);
            parameters.Add("Salt", user.Salt);

            await Task.FromResult(_dapper.Update<User>("UpdateUserPassword", parameters));
        }
        public async Task UpdateUserRefreshTokenAsync(User user)
        {
            var parameters = new DynamicParameters();
            parameters.Add("Id", user.Id);
            parameters.Add("RefreshToken", user.RefreshToken);
            parameters.Add("RefreshTokenExpiration", user.RefreshTokenExpiration);

            await Task.FromResult(_dapper.Update<User>("UpdateUserRefreshToken", parameters));
        }
        public async Task AddUpdateUserRateAsync(UserRate userRate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userRate.UserId);
            parameters.Add("ProductTypeId", userRate.ProductTypeId);
            parameters.Add("ProductSubTypeId", userRate.ProductSubTypeId);
            parameters.Add("Rate", userRate.Rate);
            parameters.Add("Percent", userRate.Percent);

            await Task.FromResult(_dapper.Update<User>("AddUpdateUserRate", parameters));
        }
        public async Task DeleteUserRateAsync(UserRate userRate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userRate.UserId);
            parameters.Add("ProductTypeId", userRate.ProductTypeId);
            parameters.Add("ProductSubTypeId", userRate.ProductSubTypeId);

            await Task.FromResult(_dapper.Delete<UserRate>("DeleteUserRate", parameters));
        }
        
        public void Dispose()
        {

        }
    }
}