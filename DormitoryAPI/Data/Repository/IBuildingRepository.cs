using System.Collections.Generic;
using System.Threading.Tasks;
using DormitoryAPI.Models;

namespace DormitoryAPI.Data.Repository
{
    public interface IBuildingRepository
    {
        Task<IEnumerable<Building>> GetAllAsync();
        Task<Building?> GetByNameAsync(string buildingName);
        Task<int> AddAsync(Building building);
        Task<int> UpdateAsync(Building building);
        Task<int> DeleteAsync(string buildingName);
    }
}