namespace DormitoryAPI.Models.ViewModels
{
    public class InvoiceDetailsView
    {
        public int InvoiceNumber { get; set; }
        public string ServiceType { get; set; } = null!;
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Remaining { get; set; }
        public string Status { get; set; } = null!;
        public string StudentName { get; set; } = null!;
        public string PassportNum { get; set; } = null!;
        public int Course { get; set; }
        public string GroupName { get; set; } = null!;
        public string BuildingName { get; set; } = null!;
        public int RoomNumber { get; set; }
    }
}