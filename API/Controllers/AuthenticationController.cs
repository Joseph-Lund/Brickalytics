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
        public async Task<LoginResponse?> Login(LoginInfo loginInfo)
        {
            try
            {
                var user = await _userService.GetUserByCreatorNameAsync(loginInfo.CreatorName!);
                if (user == null)
                {
                    throw new Exception();
                }
                if (VerifyPassword(loginInfo.Password!, user.Hash!, user.Salt!))
                {
                    var tokens = GenerateTokens(user.Id);
                    user.RefreshToken = tokens.RefreshToken;
                    user.RefreshTokenExpiration = tokens.RefreshTokenExpiration;
                    await _userService.UpdateUserRefreshTokenAsync(user);

                    return new LoginResponse(){
                        Id = user.Id,
                        CreatorName = user.CreatorName,
                        Email = user.Email,
                        AccessToken = tokens.AccessToken,
                        RefreshToken = tokens.RefreshToken,
                        RefreshTokenExpiration = tokens.RefreshTokenExpiration
                    };
                }
                else
                {
                    throw new Exception();
                }
            }
            catch
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

        }
        [HttpPut]
        [Route("Logout")]
        public async Task Logout(Tokens tokens)
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
        }
        [HttpPost]
        [Route("Refresh")]
        public async Task<Tokens?> Refresh(Tokens tokens)
        {
            var accessTokenUserId = ValidateToken(tokens.AccessToken!);
            var refreshTokenUserId = ValidateToken(tokens.RefreshToken!);
            if (accessTokenUserId != null && refreshTokenUserId != null)
            {
                var user = await _userService.GetUserByIdAsync((int)accessTokenUserId);
                return GenerateTokens(user.Id);
            }
            else
            {
                return null;
            }

        }
        private string GenerateAccessToken(int id)
        {
            // generate token that is valid for 1 minute
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", id.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private Tokens GenerateRefreshToken(int id)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
            var expires = DateTime.UtcNow.AddDays(7);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", id.ToString()) }),
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
        private Tokens GenerateTokens(int id)
        {
            var accessToken = GenerateAccessToken(id);
            var refreshToken = GenerateRefreshToken(id);

            return new Tokens()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.RefreshToken,
                RefreshTokenExpiration = refreshToken.RefreshTokenExpiration
            };
        }
    }
}
