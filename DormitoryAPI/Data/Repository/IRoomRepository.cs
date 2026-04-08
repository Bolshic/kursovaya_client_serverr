using DormitoryAPI.Models;
using DormitoryAPI.Models.ViewModels;

namespace DormitoryAPI.Data.Repository
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllAsync();
        Task<Room?> GetByIdAsync(int roomId);
        Task<IEnumerable<Room>> GetByBuildingAsync(string buildingName);
        Task<IEnumerable<FreeRoomView>> GetFreeRoomsAsync(); // из представления v_free_rooms
        Task<int> AddAsync(Room room);
        Task<int> UpdateAsync(Room room);
        Task<int> DeleteAsync(int roomId);
    }

}