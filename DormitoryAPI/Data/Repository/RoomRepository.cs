using Dapper;
using DormitoryAPI.Models;
using DormitoryAPI.Models.ViewModels;

namespace DormitoryAPI.Data.Repository
{
    public class RoomRepository : IRoomRepository
{
    private readonly DapperContext _context;
    public RoomRepository(DapperContext context) => _context = context;

    public async Task<IEnumerable<Room>> GetAllAsync()
    {
        var sql = "SELECT room_id, room_number, places_count, lockers_count, chairs_count, extra_info, building_name FROM Room";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Room>(sql);
    }

    public async Task<Room?> GetByIdAsync(int roomId)
    {
        var sql = "SELECT room_id, room_number, places_count, lockers_count, chairs_count, extra_info, building_name FROM Room WHERE room_id = @roomId";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Room>(sql, new { roomId });
    }

    public async Task<IEnumerable<Room>> GetByBuildingAsync(string buildingName)
    {
        var sql = "SELECT room_id, room_number, places_count, lockers_count, chairs_count, extra_info, building_name FROM Room WHERE building_name = @buildingName";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Room>(sql, new { buildingName });
    }

    public async Task<IEnumerable<FreeRoomView>> GetFreeRoomsAsync()
    {
        var sql = "SELECT building_name, room_number, places_count, free_places FROM v_free_rooms ORDER BY building_name, room_number";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<FreeRoomView>(sql);
    }

    public async Task<int> AddAsync(Room room)
    {
        var sql = @"INSERT INTO Room (room_number, places_count, lockers_count, chairs_count, extra_info, building_name)
                    VALUES (@RoomNumber, @PlacesCount, @LockersCount, @ChairsCount, @ExtraInfo, @BuildingName)";
        using var connection = _context.CreateConnection();
        return await connection.ExecuteAsync(sql, room);
    }

    public async Task<int> UpdateAsync(Room room)
    {
        var sql = @"UPDATE Room SET room_number = @RoomNumber, places_count = @PlacesCount,
                    lockers_count = @LockersCount, chairs_count = @ChairsCount,
                    extra_info = @ExtraInfo, building_name = @BuildingName
                    WHERE room_id = @RoomId";
        using var connection = _context.CreateConnection();
        return await connection.ExecuteAsync(sql, room);
    }

    public async Task<int> DeleteAsync(int roomId)
    {
        var sql = "DELETE FROM Room WHERE room_id = @roomId";
        using var connection = _context.CreateConnection();
        return await connection.ExecuteAsync(sql, new { roomId });
    }
}
}