using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Brickalytics.Models;
using Brickalytics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Brickalytics.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthenticationController(ILogger<AuthenticationController> logger, IConfiguration configuration, IUserService userService)
        {
            _logger = logger;
            _configuration = configuration;
            _userService = userService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<Result<LoginResponse>> Login(LoginInfo loginInfo)
        {
            try
            {
                var user = await _userService.GetUserByCreatorNameAsync(loginInfo.CreatorName!);
                if (user == null)
                {
                    throw new Exception("Invalid Username or Password");
                }
                if (VerifyPassword(loginInfo.Password!, user.Hash!, user.Salt!))
                {
                    var tokens = GenerateTokens(user.Id, user.RoleId);
                    user.RefreshToken = tokens.RefreshToken;
                    user.RefreshTokenExpiration = tokens.RefreshTokenExpiration;
                    await _userService.UpdateUserRefreshTokenAsync(user);
                    var data = new LoginResponse()
                    {
                        Id = user.Id,
                        CreatorName = user.CreatorName,
                        Email = user.Email,
                        IsAdmin = user.RoleId != (int)Roles.User,
                        AccessToken = tokens.AccessToken,
                        RefreshToken = tokens.RefreshToken,
                        RefreshTokenExpiration = tokens.RefreshTokenExpiration
                    };
                    return new Result<LoginResponse> { Code = 200, Message = "Success", Data = data };
                }
                else
                {
                    throw new Exception("Invalid Username or Password");
                }
            }
            catch (Exception ex)
            {
                return new Result<LoginResponse> { Code = 500, Message = ex.Message };
            }

        }
        [HttpPut]
        [Route("Logout")]
        public async Task<Result> Logout(Tokens tokens)
        {
            try
            {
                var accessTokenUserId = ValidateToken(tokens.AccessToken!);
                var refreshTokenUserId = ValidateToken(tokens.RefreshToken!);
                if (accessTokenUserId != null && refreshTokenUserId != null)
                {
                    var user = await _userService.GetUserByIdAsync((int)accessTokenUserId);
                    user.RefreshToken = null;
                    user.RefreshTokenExpiration = null;
                    await _userService.UpdateUserRefreshTokenAsync(user);
                }
                return new Result { Code = 200, Message = "Success" };
            }
            catch (Exception ex)
            {
                return new Result { Code = 500, Message = ex.Message };
            }
        }
        [HttpPost]
        [Route("Refresh")]
        public async Task<Result<Tokens>> Refresh(Tokens tokens)
        {
            try
            {
                //TODO: Refresh token should check with sql db if it is the same token assigned to the user last
                var accessTokenUserId = ValidateToken(tokens.AccessToken!);
                var refreshTokenUserId = ValidateToken(tokens.RefreshToken!);
                if (accessTokenUserId != null && refreshTokenUserId != null)
                {
                    var user = await _userService.GetUserByIdAsync((int)accessTokenUserId);
                    var data = GenerateTokens(user.Id, user.RoleId);
                    return new Result<Tokens> { Code = 200, Message = "Success", Data = data };
                }
                else
                {
                    return new Result<Tokens> { Code = 500, Message = "Could not validate tokens" };
                }
            }
            catch (Exception ex)
            {
                return new Result<Tokens> { Code = 500, Message = ex.Message };
            }

        }
        private string GenerateAccessToken(int userId, int roleId)
        {
            // generate token that is valid for 1 minute
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userId.ToString()), new Claim("roleId", roleId.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private Tokens GenerateRefreshToken(int userId, int roleId)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            var expires = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JWT:RefreshTokenValidityInDays"]));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", userId.ToString()), new Claim("roleId", roleId.ToString()) }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new Tokens() { AccessToken = null, RefreshToken = tokenHandler.WriteToken(token), RefreshTokenExpiration = expires };
        }
        private int? ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // return user id from JWT token if validation successful
            return userId;
        }
        private bool VerifyPassword(string userEnteredPassword, string dbPasswordHash, string dbPasswordSalt)
        {
            string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userEnteredPassword,
                salt: Convert.FromBase64String(dbPasswordSalt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
            return dbPasswordHash == hashedPassword;
        }
        private Tokens GenerateTokens(int userId, int roleId)
        {
            var accessToken = GenerateAccessToken(userId, roleId);
            var refreshToken = GenerateRefreshToken(userId, roleId);

            return new Tokens()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.RefreshToken,
                RefreshTokenExpiration = refreshToken.RefreshTokenExpiration
            };
        }
    }
}
