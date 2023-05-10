using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Brickalytics.Services;

namespace Brickalytics.Controllers
{
    [Authorize]
    [ApiController]
    [Route("Role")]
    public class RoleController : ControllerBase
    {
        private readonly ILogger<RoleController> _logger;
        private readonly IRoleService _roleService;

        public RoleController(ILogger<RoleController> logger, IRoleService roleService)
        {
            _logger = logger;
            _roleService = roleService;
        }

        [HttpGet()]
        public async Task<List<Role>> GetUsers()
        {
            var result = await _roleService.GetRolesAsync();
            return result;
        }
    }
}
