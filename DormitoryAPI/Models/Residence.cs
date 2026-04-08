namespace DormitoryAPI.Models
{
    public class Residence
    {
        public int OrderNumber { get; set; }
        public string PassportNum { get; set; } = null!;
        public int RoomId { get; set; }
        public DateTime DateIn { get; set; }
        public DateTime? DateOut { get; set; }
    }
}