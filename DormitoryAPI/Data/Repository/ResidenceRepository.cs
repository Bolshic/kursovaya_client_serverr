using Dapper;
using DormitoryAPI.Models;
using DormitoryAPI.Models.ViewModels;

namespace DormitoryAPI.Data.Repository
{
    public class ResidenceRepository : IResidenceRepository
    {
    private readonly DapperContext _context;
    public ResidenceRepository(DapperContext context) => _context = context;

    public async Task<int> SettleStudent(string passportNum, int roomId, DateTime dateIn)
    {
        var sql = "CALL settle_student(@p_passport_num, @p_room_id, @p_date_in)";
        using var connection = _context.CreateConnection();
        return await connection.ExecuteAsync(sql, new
        {
            p_passport_num = passportNum,
            p_room_id = roomId,
            p_date_in = dateIn.Date
        });
    }

    public async Task<int> EvictStudent(int orderNumber, DateTime dateOut)
    {
        var sql = "CALL evict_student(@p_order_number, @p_date_out)";
        using var connection = _context.CreateConnection();
        return await connection.ExecuteAsync(sql, new
        {
            p_order_number = orderNumber,
            p_date_out = dateOut.Date
        });
    }

    public async Task<int> RelocateStudent(int orderNumber, int newRoomId, DateTime relocationDate)
    {
        var sql = "CALL relocate_student(@p_order_number, @p_new_room_id, @p_relocation_date)";
        using var connection = _context.CreateConnection();
        return await connection.ExecuteAsync(sql, new
        {
            p_order_number = orderNumber,
            p_new_room_id = newRoomId,
            p_relocation_date = relocationDate.Date
        });
    }

    public async Task<IEnumerable<Residence>> GetHistoryByStudent(string passportNum)
    {
        var sql = "SELECT order_number, passport_num, room_id, date_in, date_out FROM Residence WHERE passport_num = @passportNum ORDER BY date_in DESC";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Residence>(sql, new { passportNum });
    }

    public async Task<IEnumerable<Residence>> GetHistoryByRoom(int roomId)
    {
        var sql = "SELECT order_number, passport_num, room_id, date_in, date_out FROM Residence WHERE room_id = @roomId ORDER BY date_in DESC";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Residence>(sql, new { roomId });
    }

    public async Task<Residence?> GetCurrentResidence(string passportNum)
    {
        var sql = "SELECT order_number, passport_num, room_id, date_in, date_out FROM Residence WHERE passport_num = @passportNum AND date_out IS NULL";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Residence>(sql, new { passportNum });
    }
    public async Task<CurrentResidenceView?> GetCurrentByPassportAsync(string passportNum)
    {
        var sql = @"
            SELECT 
                r.order_number AS OrderNumber,
                r.passport_num AS PassportNum,
                r.room_id AS RoomId,
                rm.room_number AS RoomNumber,
                b.building_name AS BuildingName,
                r.date_in::timestamp AS DateIn,
                r.date_out::timestamp AS DateOut
            FROM Residence r
            JOIN Room rm ON r.room_id = rm.room_id
            JOIN Building b ON rm.building_name = b.building_name
            WHERE r.passport_num = @passportNum 
            AND (r.date_out > CURRENT_DATE OR r.date_out IS NULL)";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<CurrentResidenceView>(sql, new { passportNum });
    }
}
}