namespace Brickalytics.Models
{
    public class ProductSoldDetail
    {
        public string? ItemName { get; set; }
        public decimal Price { get; set; }
        public decimal Share { get; set; }
        public DateTime? OrderFulfilledDate { get; set; }
    }
}