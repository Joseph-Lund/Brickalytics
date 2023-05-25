namespace Brickalytics.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public decimal PaymentAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? FormattedDate { get; set; }
        public string? FormattedTime { get; set; }
        
    }
}