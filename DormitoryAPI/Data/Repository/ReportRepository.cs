using Dapper;
using DormitoryAPI.Models;
using DormitoryAPI.Models.ViewModels;

namespace DormitoryAPI.Data.Repository
{
    public class ReportRepository : IReportRepository
{
    private readonly DapperContext _context;
    public ReportRepository(DapperContext context) => _context = context;

    public async Task<IEnumerable<FreeRoomView>> GetFreeRooms()
    {
        var sql = @"
            SELECT 
                building_name AS BuildingName,
                room_number AS RoomNumber,
                places_count AS PlacesCount,
                free_places AS FreePlaces
            FROM v_free_rooms
            ORDER BY building_name, room_number";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<FreeRoomView>(sql);
    }


    public async Task<IEnumerable<DebtView>> GetDebts()
    {
        var sql = @"
            SELECT 
                student_name AS StudentName,
                passport_num AS PassportNum,
                course AS Course,
                group_name AS GroupName,
                building_name AS BuildingName,
                room_number AS RoomNumber,
                total_debt AS TotalDebt,
                unpaid_invoices AS UnpaidInvoices
            FROM v_debts
            ORDER BY total_debt DESC";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<DebtView>(sql);
    }

    public async Task<IEnumerable<OccupancyStat>> GetOccupancyStats()
    {
        var sql = @"
            SELECT 
                building_name AS BuildingName,
                total_rooms AS TotalRooms,
                occupied_rooms AS OccupiedRooms,
                total_places AS TotalPlaces,
                occupied_places AS OccupiedPlaces,
                occupancy_percent AS OccupancyPercent
            FROM v_occupancy_stats
            ORDER BY building_name";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<OccupancyStat>(sql);
    }

    public async Task<IEnumerable<CurrentResidenceView>> GetResidentsByFaculty(string facultyName)
    {
        var sql = "SELECT * FROM v_faculty_students_with_rooms WHERE faculty_name = @facultyName";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<CurrentResidenceView>(sql, new { facultyName });
    }

    public async Task<object> GetMonthlyPayments(int year)
    {
        var sql = @"
            SELECT 
                EXTRACT(MONTH FROM pay_date) AS month,
                SUM(amount) AS total_paid
            FROM Payment p
            JOIN Invoice i ON p.invoice_number = i.invoice_number
            WHERE EXTRACT(YEAR FROM pay_date) = @year
            GROUP BY EXTRACT(MONTH FROM pay_date)
            ORDER BY month";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync(sql, new { year });
    }
}
}