namespace DormitoryAPI.Models.ViewModels
{
    public class OccupancyStat
    {
        public string? BuildingName { get; set; }
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int TotalPlaces { get; set; }
        public int OccupiedPlaces { get; set; }
        public double OccupancyPercent { get; set; }
    }
}