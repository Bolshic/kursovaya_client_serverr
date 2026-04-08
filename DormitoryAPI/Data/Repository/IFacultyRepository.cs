using DormitoryAPI.Models;

namespace DormitoryAPI.Data.Repository
{
    public interface IFacultyRepository
    {
        Task<IEnumerable<Faculty>> GetAllAsync();
        Task<Faculty?> GetByNameAsync(string facultyName);
        Task<int> AddAsync(Faculty faculty);
        Task<int> UpdateAsync(Faculty faculty);
        Task<int> DeleteAsync(string facultyName);
    }
}