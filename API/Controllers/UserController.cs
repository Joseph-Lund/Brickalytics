using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Brickalytics.Services;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Brickalytics.Models;
using Microsoft.Net.Http.Headers;
using Brickalytics.Helpers;

namespace Brickalytics.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly ITokenHelper _tokenHelper;
        private readonly string _accessToken;

        public UserController(ILogger<UserController> logger, IUserService userService, ITokenHelper tokenHelper)
        {
            _logger = logger;
            _userService = userService;
            _tokenHelper = tokenHelper;
            _accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
        }

        [HttpGet]
        public async Task<List<User>> GetUsers()
        {
            if(_tokenHelper.IsUserAdmin(_accessToken))
            {
                var result = await _userService.GetUsersAsync();
                return result;
            }
            throw new UnauthorizedAccessException();
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
            if(_tokenHelper.IsUserAdmin(_accessToken))
            {
                var result = await _userService.AddUserAsync(user);
                return result;
            }
            throw new UnauthorizedAccessException();
        }
        [HttpPut]
        public async Task UpdateUser(User user)
        {
            if(_tokenHelper.IsUserAdmin(_accessToken))
            {
                await _userService.UpdateUserAsync(user);
            }
            throw new UnauthorizedAccessException();
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
            if(_tokenHelper.IsUserAdmin(_accessToken))
            {
                await _userService.AddUpdateUserRateAsync(userRate);
            }
            throw new UnauthorizedAccessException();
        }
        [HttpDelete]
        [Route("Rate")]
        public async Task DeleteUserRate(UserRate userRate)
        {
            if(_tokenHelper.IsUserAdmin(_accessToken))
            {
                await _userService.DeleteUserRateAsync(userRate);
            }
            throw new UnauthorizedAccessException();
        }
    }
}
