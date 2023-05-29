using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Brickalytics.Services;
using Brickalytics.Models;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Brickalytics.Helpers;

namespace Brickalytics.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleService _roleService;
        private readonly ITokenHelper _tokenHelper;

        public RoleController(ILogger<RoleController> logger, IRoleService roleService, ITokenHelper tokenHelper)
        {
            _logger = logger;
            _roleService = roleService;
            _tokenHelper = tokenHelper;
        }

        [HttpGet]
        public async Task<Result<List<Role>>> GetRoles()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
            try
            {
                if (_tokenHelper.IsUserAdmin(accessToken))
                {
                    var data = await _roleService.GetRolesAsync();
                    return new Result<List<Role>>() { Code = 200, Message = "Success", Data = data };
                }
                else
                {
                    throw new UnauthorizedAccessException("Not authorized to get Roles");
                }
            }
            catch (Exception ex)
            {

                return new Result<List<Role>>() { Code = 500, Message = ex.Message };
            }
        }
    }
}
