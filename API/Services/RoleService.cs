using Brickalytics.Helpers;
using Brickalytics.Models;

namespace Brickalytics.Services
{
    public class RoleService : IRoleService
    {
        private readonly ILogger<RoleService> _logger;
        private readonly IDapperHelper _dapper;

        public RoleService(ILogger<RoleService> logger, IDapperHelper dapper)
        {
            _logger = logger;
            _dapper = dapper;
        }
        public async Task<List<Role>> GetRolesAsync()
        {
            var result = await Task.FromResult(_dapper.GetAll<Role>("GetRoles"));
            return result;
        }
        
        public void Dispose()
        {

        }
    }
}