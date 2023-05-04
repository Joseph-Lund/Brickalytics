namespace Brickalytics
{
    public class User
    {
        public int Id {get; set;}
        public string? CreatorName { get; set; }
        public string? Email { get; set; }
        public decimal Rate {get; set; }
        public bool Active { get; set; }
        public string? Hash { get; set; }
        public string? Salt { get; set; }
        public int RoleId {get; set;}
        public string? RefreshToken {get; set;}
        public DateTime? RefreshTokenExpiration {get; set;}
    }
}