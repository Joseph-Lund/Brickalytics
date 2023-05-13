namespace Brickalytics.Models
{
    public class LoginResponse: Tokens
    {
        public int? Id { get; set; }
        public string? CreatorName {get; set;}
        public string? Email { get; set; }
    }
}