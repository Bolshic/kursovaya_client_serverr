namespace DormitoryAPI.Models.ViewModels
{
    public class FreeRoomView
    {
        public string? BuildingName { get; set; }
        public int RoomNumber { get; set; }
        public int PlacesCount { get; set; }
        public int FreePlaces { get; set; }
    }
}