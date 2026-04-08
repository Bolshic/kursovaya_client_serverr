using DormitoryAPI.Models;
using DormitoryAPI.Models.ViewModels;


namespace DormitoryAPI.Data.Repository
{
    public interface IReportRepository
{
    Task<IEnumerable<FreeRoomView>> GetFreeRooms();
    Task<IEnumerable<DebtView>> GetDebts(); 
    Task<IEnumerable<OccupancyStat>> GetOccupancyStats();
    Task<IEnumerable<CurrentResidenceView>> GetResidentsByFaculty(string facultyName); // есть в StudentRepository, но можно продублировать
    Task<object> GetMonthlyPayments(int year); 
}

}