using Dapper;
using DormitoryAPI.Models;
using DormitoryAPI.Models.ViewModels;

namespace DormitoryAPI.Data.Repository
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DapperContext _context;
        public StudentRepository(DapperContext context) => _context = context;

        // Старый метод для совместимости
        public async Task<IEnumerable<Student>> GetAllAsync(string? faculty = null, int? course = null, string? group = null, bool? onlyResidents = null)
        {
            var sql = @"
                SELECT 
                    s.passport_num AS PassportNum,
                    s.fio AS Fio,
                    s.group_name AS GroupName
                FROM Student s";
            var conditions = new List<string>();
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(faculty) || course.HasValue || !string.IsNullOrEmpty(group))
            {
                sql += " JOIN \"Group\" g ON s.group_name = g.group_name";
            }

            if (!string.IsNullOrEmpty(faculty))
            {
                conditions.Add("g.faculty_name = @faculty");
                parameters.Add("@faculty", faculty);
            }
            if (course.HasValue)
            {
                conditions.Add("g.course = @course");
                parameters.Add("@course", course.Value);
            }
            if (!string.IsNullOrEmpty(group))
            {
                conditions.Add("s.group_name = @group");
                parameters.Add("@group", group);
            }
            if (onlyResidents == true)
            {
                sql += " JOIN Residence r ON s.passport_num = r.passport_num WHERE (r.date_out > CURRENT_DATE OR r.date_out IS NULL)";
            }
            else if (conditions.Any())
            {
                sql += " WHERE " + string.Join(" AND ", conditions);
            }

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<Student>(sql, parameters);
        }

        // НОВЫЙ МЕТОД для фильтрации с комнатой, корпусом и поиском по ФИО
        public async Task<IEnumerable<StudentWithRoomView>> GetAllWithFiltersAsync(string? faculty = null, int? course = null, string? group = null, string? search = null)
        {
            var sql = @"
                SELECT 
                    s.passport_num AS PassportNum,
                    s.fio AS Fio,
                    s.group_name AS GroupName,
                    r.room_number AS RoomNumber,
                    b.building_name AS BuildingName,
                    res.order_number AS OrderNumber
                FROM Student s
                LEFT JOIN Residence res ON s.passport_num = res.passport_num AND (res.date_out > CURRENT_DATE OR res.date_out IS NULL)
                LEFT JOIN Room r ON res.room_id = r.room_id
                LEFT JOIN Building b ON r.building_name = b.building_name
                LEFT JOIN ""Group"" g ON s.group_name = g.group_name
                WHERE 1=1
            ";
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(faculty))
            {
                sql += " AND g.faculty_name = @faculty";
                parameters.Add("@faculty", faculty);
            }
            if (course.HasValue)
            {
                sql += " AND g.course = @course";
                parameters.Add("@course", course.Value);
            }
            if (!string.IsNullOrEmpty(group))
            {
                sql += " AND s.group_name = @group";
                parameters.Add("@group", group);
            }
            if (!string.IsNullOrEmpty(search))
            {
                sql += " AND s.fio ILIKE @search";
                parameters.Add("@search", $"%{search}%");
            }

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<StudentWithRoomView>(sql, parameters);
        }

        public async Task<Student?> GetByPassportAsync(string passportNum)
        {
            var sql = "SELECT passport_num AS PassportNum, fio AS Fio, group_name AS GroupName FROM Student WHERE passport_num = @passportNum";
            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<Student>(sql, new { passportNum });
        }

        public async Task<IEnumerable<CurrentResidenceView>> GetCurrentResidenceByFaculty(string facultyName)
        {
            var sql = @"
                SELECT 
                    passport_num AS PassportNum,
                    fio AS Fio,
                    course AS Course,
                    group_name AS GroupName,
                    faculty_name AS FacultyName,
                    order_number AS OrderNumber,
                    building_name AS BuildingName,
                    building_address AS BuildingAddress,
                    room_number AS RoomNumber,
                    date_in::timestamp AS DateIn
                FROM v_faculty_students_with_rooms 
                WHERE faculty_name = @facultyName";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<CurrentResidenceView>(sql, new { facultyName });
        }

        public async Task<IEnumerable<CurrentResidenceView>> GetCurrentResidents()
        {
            var sql = @"
                SELECT 
                    passport_num AS PassportNum,
                    fio AS Fio,
                    course AS Course,
                    group_name AS GroupName,
                    faculty_name AS FacultyName,
                    order_number AS OrderNumber,
                    building_name AS BuildingName,
                    building_address AS BuildingAddress,
                    room_number AS RoomNumber,
                    date_in::timestamp AS DateIn
                FROM v_current_residence";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<CurrentResidenceView>(sql);
        }

        public async Task<int> AddAsync(Student student)
        {
            var sql = "INSERT INTO Student (passport_num, fio, group_name) VALUES (@PassportNum, @Fio, @GroupName)";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, student);
        }

        public async Task<int> UpdateAsync(Student student)
        {
            var sql = "UPDATE Student SET fio = @Fio, group_name = @GroupName WHERE passport_num = @PassportNum";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, student);
        }

        public async Task<int> DeleteAsync(string passportNum)
        {
            var sql = "DELETE FROM Student WHERE passport_num = @passportNum";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(sql, new { passportNum });
        }
    }
}