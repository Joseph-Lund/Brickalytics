namespace Brickalytics.Models
{
    public class Order
    {
        public long ProductId {get; set;}
        public int Count { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public ProductTypes ProductType { get; set; }
        
    }
}