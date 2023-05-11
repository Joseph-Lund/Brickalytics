namespace Brickalytics.Models
{
    public class UserRate
    {
        public int UserId {get; set;}
        public int? ProductTypeId {get; set;}
        public int ProductSubTypeId {get; set;}
        public decimal? Rate { get; set; }
        public decimal? Percent { get; set; }
    }
}