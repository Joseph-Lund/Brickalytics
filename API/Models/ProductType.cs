namespace Brickalytics.Models
{
    public class ProductType
    {
        public int Id {get; set;}
        public string? CreatorName { get; set; }
        public string? Email { get; set; }
        public bool Active { get; set; }
        public string? Hash { get; set; }
        public string? Salt { get; set; }
        public int RoleId {get; set;}
        public string? RefreshToken {get; set;}
        public DateTime? RefreshTokenExpiration {get; set;}
    }
}