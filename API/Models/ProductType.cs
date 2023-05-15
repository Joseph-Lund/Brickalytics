namespace Brickalytics.Models
{
    public class ProductType
    {
        public int Id {get; set;}
        public string? Name { get; set; }
    }
    public enum ProductTypes
    {
        Tee = 1,
        Print = 2,
        Hoodie = 3,
        Stricker = 4,
        BodyPillow = 5
    }
}