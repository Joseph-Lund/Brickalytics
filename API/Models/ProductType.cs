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
        Hoodie = 2,
        Print = 3,
        Stickers = 4,
        Pillow = 5
    }
}