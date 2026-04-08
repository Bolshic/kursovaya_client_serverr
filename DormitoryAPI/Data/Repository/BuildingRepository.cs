using Dapper;
using DormitoryAPI.Models;

namespace DormitoryAPI.Data.Repository
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly DapperContext _context;
        public BuildingRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<Building>> GetAllAsync()
        {
            var sql = "SELECT building_name AS BuildingName, address AS Address FROM Building";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Building>(sql);
        }

        public async Task<Building?> GetByNameAsync(string buildingName)
        {
            var sql = "SELECT building_name AS BuildingName, address AS Address FROM Building WHERE building_name = @buildingName";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Building>(sql, new { buildingName });
        }

        public async Task<int> AddAsync(Building building)
        {
            var sql = "INSERT INTO Building (building_name, address) VALUES (@BuildingName, @Address)";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, building);
        }

        public async Task<int> UpdateAsync(Building building)
        {
            var sql = "UPDATE Building SET address = @Address WHERE building_name = @BuildingName";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, building);
        }

        public async Task<int> DeleteAsync(string buildingName)
        {
            var sql = "DELETE FROM Building WHERE building_name = @buildingName";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, new { buildingName });
        }
    }
}