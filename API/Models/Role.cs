namespace Brickalytics.Models
{
    public class Role
    {
        public int Id {get; set;}
        public string? Name { get; set; }
    }
    
    public enum Roles
    {
        Admin = 1,
        User = 2,
        Dev = 3
    }
}