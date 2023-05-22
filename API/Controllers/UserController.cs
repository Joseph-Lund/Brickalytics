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
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        private readonly IShopifyService _shopifyService;
        private readonly ITokenHelper _tokenHelper;
        IRoleService _roleService;

        public UserController(ILogger<UserController> logger, IUserService userService, IRoleService roleService, IShopifyService shopifyService, ITokenHelper tokenHelper)
        {
            _logger = logger;
            _userService = userService;
            _tokenHelper = tokenHelper;
            _shopifyService = shopifyService;
            _roleService= roleService;
        }

        [HttpGet]
        [Route("Names")]
        public async Task<List<CreatorNameListItem>> GetCreatorNames()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            if(_tokenHelper.IsUserAdmin(accessToken))
            {   
                List<User> users = await GetUsers();
                List<CreatorNameListItem> result = users.Where(user => user.CollectionId != 0).Select(user => new CreatorNameListItem(){ Name = user.CreatorName, Id = user.Id}).ToList();
                return result;
            }
            throw new UnauthorizedAccessException();
        }
        [HttpGet]
        public async Task<List<User>> GetUsers()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            if(_tokenHelper.IsUserAdmin(accessToken))
            {   await _userService.GetUsersAsync();
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
        [HttpGet]
        [Route("Collections")]
        public async Task<List<Collection>> GetShopifyCollections(int id)
        {
            var result = await _shopifyService.GetCollectionsAsync();
            return result;
        }
        [HttpPost]
        public async Task<int> AddUser(User user)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            if(_tokenHelper.IsUserAdmin(accessToken))
            {
                var result = await _userService.AddUserAsync(user);
                return result;
            }
            throw new UnauthorizedAccessException();
        }
        [HttpPut]
        public async Task UpdateUser(User user)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            if(_tokenHelper.IsUserAdmin(accessToken))
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
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            if(_tokenHelper.IsUserAdmin(accessToken))
            {
                await _userService.AddUpdateUserRateAsync(userRate);
            }
            throw new UnauthorizedAccessException();
        }
        [HttpDelete]
        [Route("Rate")]
        public async Task DeleteUserRate(UserRate userRate)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            if(_tokenHelper.IsUserAdmin(accessToken))
            {
                await _userService.DeleteUserRateAsync(userRate);
            }
            throw new UnauthorizedAccessException();
        }
        [HttpGet]
        [Route("Roles")]
        public async Task<List<Role>> GetRoles()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            if(_tokenHelper.IsUserAdmin(accessToken))
            {
                var result = await _roleService.GetRolesAsync();
                return result;
            }
            throw new UnauthorizedAccessException();
        }
    }
}
