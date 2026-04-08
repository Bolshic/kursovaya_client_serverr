using Dapper;
using DormitoryAPI.Models;

namespace DormitoryAPI.Data.Repository
{
    public class GroupRepository : IGroupRepository
    {
        private readonly DapperContext _context;
        public GroupRepository(DapperContext context) => _context = context;

        public async Task<IEnumerable<Group>> GetAllAsync()
        {
            var sql = @"
                SELECT 
                    group_name AS GroupName, 
                    faculty_name AS FacultyName, 
                    course AS Course 
                FROM ""Group""";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Group>(sql);
        }

        public async Task<Group?> GetByNameAsync(string groupName)
        {
            var sql = @"
                SELECT 
                    group_name AS GroupName, 
                    faculty_name AS FacultyName, 
                    course AS Course 
                FROM ""Group"" 
                WHERE group_name = @groupName";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Group>(sql, new { groupName });
        }

        public async Task<IEnumerable<Group>> GetByFacultyAsync(string facultyName)
        {
            var sql = @"
                SELECT 
                    group_name AS GroupName, 
                    faculty_name AS FacultyName, 
                    course AS Course 
                FROM ""Group"" 
                WHERE faculty_name = @facultyName";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Group>(sql, new { facultyName });
        }

        public async Task<int> AddAsync(Group group)
        {
            var sql = "INSERT INTO \"Group\" (group_name, faculty_name, course) VALUES (@GroupName, @FacultyName, @Course)";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, group);
        }

        public async Task<int> UpdateAsync(Group group)
        {
            var sql = "UPDATE \"Group\" SET faculty_name = @FacultyName, course = @Course WHERE group_name = @GroupName";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, group);
        }

        public async Task<int> DeleteAsync(string groupName)
        {
            var sql = "DELETE FROM \"Group\" WHERE group_name = @groupName";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, new { groupName });
        }
    }
}