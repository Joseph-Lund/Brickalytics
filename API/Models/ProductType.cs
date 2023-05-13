namespace Brickalytics.Models
{
    public class ProductType
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
    public enum ProductTypes
    {
        Tee,
        Print,
        Hoodie,
        Stricker,
        BodyPillow
    }
}