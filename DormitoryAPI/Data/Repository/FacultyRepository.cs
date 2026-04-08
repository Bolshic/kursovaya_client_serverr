using Dapper;
using DormitoryAPI.Models;

namespace DormitoryAPI.Data.Repository
{
    public class FacultyRepository : IFacultyRepository
    {
        private readonly DapperContext _context;
        public FacultyRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<Faculty>> GetAllAsync()
        {
            var sql = "SELECT faculty_name AS FacultyName FROM Faculty";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Faculty>(sql);
        }

        public async Task<Faculty?> GetByNameAsync(string facultyName)
        {
            var sql = "SELECT faculty_name AS FacultyName FROM Faculty WHERE faculty_name = @facultyName";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Faculty>(sql, new { facultyName });
        }

        public async Task<int> AddAsync(Faculty faculty)
        {
            var sql = "INSERT INTO Faculty (faculty_name) VALUES (@FacultyName)";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, faculty);
        }

        public async Task<int> UpdateAsync(Faculty faculty)
        {
            var sql = "UPDATE Faculty SET faculty_name = @FacultyName WHERE faculty_name = @FacultyName"; // бессмысленно, но оставим
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, faculty);
        }

        public async Task<int> DeleteAsync(string facultyName)
        {
            var sql = "DELETE FROM Faculty WHERE faculty_name = @facultyName";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, new { facultyName });
        }
    }
}