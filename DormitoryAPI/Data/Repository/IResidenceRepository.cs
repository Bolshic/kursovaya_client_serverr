using DormitoryAPI.Models;

namespace DormitoryAPI.Data.Repository
{
    public interface IResidenceRepository
{
    Task<int> SettleStudent(string passportNum, int roomId, DateTime dateIn);
    Task<int> EvictStudent(int orderNumber, DateTime dateOut);
    Task<int> RelocateStudent(int orderNumber, int newRoomId, DateTime relocationDate);
    Task<IEnumerable<Residence>> GetHistoryByStudent(string passportNum);
    Task<IEnumerable<Residence>> GetHistoryByRoom(int roomId);
    Task<Residence?> GetCurrentResidence(string passportNum);
}

}