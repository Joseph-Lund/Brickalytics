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
        private readonly IProductTypeService _productTypeService;
        private readonly IRoleService _roleService;

        public UserController(ILogger<UserController> logger, IUserService userService, IRoleService roleService, IShopifyService shopifyService, ITokenHelper tokenHelper, IProductTypeService productTypeService)
        {
            _logger = logger;
            _userService = userService;
            _tokenHelper = tokenHelper;
            _shopifyService = shopifyService;
            _roleService = roleService;
            _productTypeService = productTypeService;
        }

        [HttpGet]
        [Route("Names")]
        public async Task<Result<List<CreatorNameListItem>>> GetCreatorNames()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);

            try
            {
                if (_tokenHelper.IsUserAdmin(accessToken))
                {
                    List<User> users = (await GetUsers()).Data!;
                    List<CreatorNameListItem> data = users.Where(user => user.CollectionId != 0).Select(user => new CreatorNameListItem() { Name = user.CreatorName, Id = user.Id }).ToList();
                    return new Result<List<CreatorNameListItem>>() { Code = 200, Message = "Success", Data = data };
                }
                throw new UnauthorizedAccessException();
            }
            catch (Exception ex)
            {

                return new Result<List<CreatorNameListItem>>() { Code = 500, Message = ex.Message };
            }
        }
        [HttpGet]
        public async Task<Result<List<User>>> GetUsers()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            try
            {
                if (_tokenHelper.IsUserAdmin(accessToken))
                {
                    var data = await _userService.GetUsersAsync();
                    return new Result<List<User>>() { Code = 200, Message = "Success", Data = data };
                }
                else
                {
                    throw new UnauthorizedAccessException("Not authorized to get User List");
                }
            }
            catch (Exception ex)
            {

                return new Result<List<User>>() { Code = 500, Message = ex.Message };
            }
        }
        [HttpGet]
        [Route("{id:int}")]
        public async Task<Result<User>> GetUserById(int id)
        {
            try
            {
                var data = await _userService.GetUserByIdAsync(id);
                return new Result<User>() { Code = 200, Message = "Success", Data = data };
            }
            catch (Exception ex)
            {

                return new Result<User>() { Code = 500, Message = ex.Message };
            }
        }
        [HttpGet]
        [Route("Collections")]
        public async Task<Result<List<Collection>>> GetShopifyCollections(int id)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            try
            {
                if (_tokenHelper.IsUserAdmin(accessToken))
                {
                    var data = await _shopifyService.GetCollectionsAsync();
                    return new Result<List<Collection>>() { Code = 200, Message = "Success", Data = data };
                }
                else
                {
                    throw new UnauthorizedAccessException("Not authorized to get Shopify Collections");
                }
            }
            catch (Exception ex)
            {

                return new Result<List<Collection>>() { Code = 500, Message = ex.Message };
            }
        }
        [HttpPost]
        public async Task<Result<int>> AddUser(User user)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            try
            {
                if (_tokenHelper.IsUserAdmin(accessToken))
                {
                    var data = await _userService.AddUserAsync(user);
                    return new Result<int>() { Code = 200, Message = "Success", Data = data }; ;
                }
                throw new UnauthorizedAccessException();
            }
            catch (Exception ex)
            {

                return new Result<int>() { Code = 500, Message = ex.Message };
            }
        }
        [HttpPut]
        public async Task<Result> UpdateUser(User user)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            try
            {
                if (_tokenHelper.IsUserAdmin(accessToken))
                {
                    await _userService.UpdateUserAsync(user);

                    return new Result() { Code = 200, Message = "Success" };
                }
                throw new UnauthorizedAccessException();
            }
            catch (Exception ex)
            {

                return new Result() { Code = 500, Message = ex.Message };
            }
        }
        [HttpPut]
        [Route("Password")]
        public async Task<Result> UpdateUserPassword(ChangePasswordInfo passwordInfo)
        {
            try
            {
                var user = await GetUserById(passwordInfo.Id);

                byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
                string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: passwordInfo.Password!,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                user.Data.Hash = hash;
                user.Data.Salt = Convert.ToBase64String(salt);

                await _userService.UpdateUserPasswordAsync(user.Data);

                return new Result() { Code = 200, Message = "Success" };
            }
            catch (Exception ex)
            {

                return new Result() { Code = 500, Message = ex.Message };
            }
        }
        [HttpPut]
        [Route("Rate")]
        public async Task<Result> AddUpdateUserRate(UserRate userRate)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            try
            {
                if (_tokenHelper.IsUserAdmin(accessToken))
                {
                    await _userService.AddUpdateUserRateAsync(userRate);
                    return new Result() { Code = 200, Message = "Success" };
                }
                throw new UnauthorizedAccessException();
            }
            catch (Exception ex)
            {
                return new Result() { Code = 500, Message = ex.Message };
            }
        }
        [HttpDelete]
        [Route("Rate")]
        public async Task<Result> DeleteUserRate(UserRate userRate)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            try
            {
                if (_tokenHelper.IsUserAdmin(accessToken))
                {
                    await _userService.DeleteUserRateAsync(userRate);
                    return new Result() { Code = 200, Message = "Success" };
                }
                throw new UnauthorizedAccessException();
            }
            catch (Exception ex)
            {

                return new Result() { Code = 500, Message = ex.Message };
            }
        }
        [HttpGet]
        [Route("Roles")]
        public async Task<Result<List<Role>>> GetRoles()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            try
            {
                if (_tokenHelper.IsUserAdmin(accessToken))
                {
                    var data = await _roleService.GetRolesAsync();
                    return new Result<List<Role>>() { Code = 500, Message = "Success", Data = data };
                }
                throw new UnauthorizedAccessException();
            }
            catch (Exception ex)
            {

                return new Result<List<Role>>() { Code = 500, Message = ex.Message };
            }
        }
        [HttpGet]
        [Route("ProductTypes")]
        public async Task<Result<List<ProductType>>> GetProductTypes()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            try
            {
                if (_tokenHelper.IsUserAdmin(accessToken))
                {
                    var data = await _productTypeService.GetProductTypesAsync();
                    return new Result<List<ProductType>>() { Code = 500, Message = "Success", Data = data };
                }
                throw new UnauthorizedAccessException();
            }
            catch (Exception ex)
            {

                return new Result<List<ProductType>>() { Code = 500, Message = ex.Message };
            }
        }
        [HttpGet]
        [Route("Rate/{userId:int}")]
        public async Task<Result<List<UserRate>>> GetUserRates(int userId)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            try
            {

                var admin = await _userService.GetUserByIdAsync(_tokenHelper.GetUserId(accessToken));
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    throw new Exception();
                }
                if (admin.IsAdmin != true)
                {
                    var adminData = await _userService.GetUserRatesAsync(admin);
                    return new Result<List<UserRate>>() { Code = 200, Message = "Success", Data = adminData };
                }
                var userData = await _userService.GetUserRatesAsync(user);
                return new Result<List<UserRate>>() { Code = 200, Message = "Success", Data = userData };
            }
            catch (Exception ex)
            {

                return new Result<List<UserRate>>() { Code = 500, Message = ex.Message };
            }
        }
    }
}
