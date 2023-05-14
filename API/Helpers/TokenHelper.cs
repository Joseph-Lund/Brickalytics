using System.IdentityModel.Tokens.Jwt;
using Brickalytics.Models;
using Microsoft.Net.Http.Headers;

namespace Brickalytics.Helpers
{
    public class TokenHelper : ITokenHelper
    {
        private readonly ILogger<TokenHelper> _logger;

        public TokenHelper(ILogger<TokenHelper> logger)
        {
            _logger = logger;
        }

        public bool IsUserAdmin(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            int roleId = Convert.ToInt32(jwt.Claims.First(c => c.Type == "roleId").Value);
            return (roleId == (int)Roles.Admin || roleId == (int)Roles.Dev);
        }
        public int GetUserId(string token)
        {
            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            int userId = Convert.ToInt32(jwt.Claims.First(c => c.Type == "id").Value);
            return userId;
        }
        
        public void Dispose()
        {

        }
    }
}