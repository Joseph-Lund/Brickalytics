namespace Brickalytics.Models
{
    public class ProductSoldParent
    {
        public decimal ProductsSoldProfit { get; set; }
        public int ProductsSoldTotal { get; set; }
        public List<ProductSoldChild>? Items { get; set; }
    }
}