using System;

namespace DormitoryAPI.Models.ViewModels
{
    public class CurrentResidenceView
    {
        public string? PassportNum { get; set; }
        public string? Fio { get; set; }
        public int Course { get; set; }
        public string? GroupName { get; set; }
        public string? FacultyName { get; set; }
        public int OrderNumber { get; set; }
        public string? BuildingName { get; set; }
        public string? BuildingAddress { get; set; }
        public int RoomNumber { get; set; }
        public DateTime DateIn { get; set; }
    }
}