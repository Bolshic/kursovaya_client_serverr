namespace DormitoryAPI.Models
{
    public class Room
    {
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }
        public int PlacesCount { get; set; }
        public int LockersCount { get; set; }
        public int ChairsCount { get; set; }
        public string? ExtraInfo { get; set; }
        public string BuildingName { get; set; } = null!;
    }
}