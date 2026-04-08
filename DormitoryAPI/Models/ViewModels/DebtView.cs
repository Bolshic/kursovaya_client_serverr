namespace DormitoryAPI.Models.ViewModels
{
    public class DebtView
    {
        public string? StudentName { get; set; }
        public string? PassportNum { get; set; }
        public int Course { get; set; }
        public string? GroupName { get; set; }
        public string? BuildingName { get; set; }
        public int RoomNumber { get; set; }
        public decimal TotalDebt { get; set; }
        public int UnpaidInvoices { get; set; }
    }
}