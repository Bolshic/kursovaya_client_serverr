namespace DormitoryAPI.Models
{
    public class Payment
    {
        public int PaymentNumber { get; set; }
        public int InvoiceNumber { get; set; }
        public DateTime PayDate { get; set; }
        public decimal Amount { get; set; }
    }
}