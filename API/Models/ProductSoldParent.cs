namespace Brickalytics.Models
{
    public class ProductSoldParent
    {
        public int ProductsSoldTotal { get; set; }
        public List<ProductSoldChild>? Items { get; set; }
    }
}