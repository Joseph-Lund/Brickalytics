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
        private readonly string _accessToken;

        public RoleController(ILogger<RoleController> logger, IRoleService roleService, ITokenHelper tokenHelper)
        {
            _logger = logger;
            _roleService = roleService;
            _tokenHelper = tokenHelper;
            _accessToken = Request.Headers[HeaderNames.Authorization].ToString().Substring(7);
        }

        [HttpGet]
        public async Task<List<Role>> GetRoles()
        {
            if(_tokenHelper.IsUserAdmin(_accessToken))
            {
                var result = await _roleService.GetRolesAsync();
                return result;
            }
            throw new UnauthorizedAccessException();
        }
    }
}
