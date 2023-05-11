namespace Brickalytics.Models
{
    public class Tokens
    {
        public string? AccessToken {get; set;}
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
    }
}