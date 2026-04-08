using DormitoryAPI.Models;

namespace DormitoryAPI.Data.Repository
{
    public interface IGroupRepository
    {
        Task<IEnumerable<Group>> GetAllAsync();
        Task<Group?> GetByNameAsync(string groupName);
        Task<IEnumerable<Group>> GetByFacultyAsync(string facultyName);
        Task<int> AddAsync(Group group);
        Task<int> UpdateAsync(Group group);
        Task<int> DeleteAsync(string groupName);
    }

}