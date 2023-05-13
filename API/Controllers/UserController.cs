using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Brickalytics.Services;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Brickalytics.Models;

namespace Brickalytics.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public async Task<List<User>> GetUsers()
        {
            var result = await _userService.GetUsersAsync();
            return result;
        }
        [HttpGet]
        [Route("{id:int}")]
        public async Task<User> GetUserById(int id)
        {
            var result = await _userService.GetUserByIdAsync(id);
            return result;
        }
        [HttpPost]
        public async Task<int> AddUser(User user)
        {
            var result = await _userService.AddUserAsync(user);
            return result;
        }
        [HttpPut]
        public async Task UpdateUser(User user)
        {
            await _userService.UpdateUserAsync(user);
        }
        [HttpPut]
        [Route("Password")]
        public async Task UpdateUserPassword(ChangePasswordInfo passwordInfo)
        {

            var user = await GetUserById(passwordInfo.Id);

            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: passwordInfo.Password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            user.Hash = hash;
            user.Salt = Convert.ToBase64String(salt);
            
            await _userService.UpdateUserPasswordAsync(user);
        }
        [HttpPut]
        [Route("Rate")]
        public async Task AddUpdateUserRate(UserRate userRate)
        {
            await _userService.AddUpdateUserRateAsync(userRate);
        }
        [HttpDelete]
        [Route("Rate")]
        public async Task DeleteUserRate(UserRate userRate)
        {
            await _userService.DeleteUserRateAsync(userRate);
        }
    }
}
