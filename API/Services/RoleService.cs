using Microsoft.AspNetCore.Mvc;
using Brickalytics.Services;

namespace Brickalytics.Controllers
{
    public class RoleService : ControllerBase
    {
        private readonly ILogger<RoleService> _logger;
        private readonly IDapperService _dapper;

        public RoleService(ILogger<RoleService> logger, IDapperService dapper)
        {
            _logger = logger;
            _dapper = dapper;
        }
        public async Task<List<Role>> GetRolesAsync()
        {
            var result = await Task.FromResult(_dapper.GetAll<Role>("GetRoles"));
            return result;
        }
    }
}