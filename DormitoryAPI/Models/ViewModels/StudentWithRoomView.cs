namespace DormitoryAPI.Models.ViewModels
{
    public class StudentWithRoomView
{
        public string? PassportNum { get; set; }
        public string? Fio { get; set; }
        public string? GroupName { get; set; }
        public int? RoomNumber { get; set; }
        public string? BuildingName { get; set; }
        public int? OrderNumber { get; set; }  // добавить
    }
}