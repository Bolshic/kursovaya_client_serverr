using DormitoryAPI.Models;
using DormitoryAPI.Models.ViewModels;

namespace DormitoryAPI.Data.Repository
{
    public interface IStudentRepository
    {
        // Старый метод (для совместимости)
        Task<IEnumerable<Student>> GetAllAsync(string? faculty = null, int? course = null, string? group = null, bool? onlyResidents = null);
        
        // Новый метод для фильтрации с комнатой/корпусом и поиском по ФИО
        Task<IEnumerable<StudentWithRoomView>> GetAllWithFiltersAsync(string? faculty = null, int? course = null, string? group = null, string? search = null);
        
        Task<Student?> GetByPassportAsync(string passportNum);
        Task<IEnumerable<CurrentResidenceView>> GetCurrentResidenceByFaculty(string facultyName);
        Task<IEnumerable<CurrentResidenceView>> GetCurrentResidents();
        Task<int> AddAsync(Student student);
        Task<int> UpdateAsync(Student student);
        Task<int> DeleteAsync(string passportNum);
    }
}