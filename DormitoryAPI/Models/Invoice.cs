namespace DormitoryAPI.Models
{
    public class Invoice
    {
        public int InvoiceNumber { get; set; }
        public int OrderNumber { get; set; }
        public string ServiceType { get; set; } = null!;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public string Status { get; set; } = "Не оплачена";
        public DateTime InvoiceDate { get; set; }
    }
}