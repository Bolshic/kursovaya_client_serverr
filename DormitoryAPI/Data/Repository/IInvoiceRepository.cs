using DormitoryAPI.Models;
using DormitoryAPI.Models.ViewModels;

namespace DormitoryAPI.Data.Repository
{
    public interface IInvoiceRepository
{
    Task<int> GenerateMonthlyInvoices(int year, int month);
    Task<int> CreateLaundryInvoice(int orderNumber, int quantity, DateTime serviceDate);
    Task<int> MakePayment(int invoiceNumber, decimal amount, DateTime payDate);
    Task<IEnumerable<InvoiceDetailsView>> GetInvoices(string? passportNum = null, string? status = null);
    Task<IEnumerable<DebtView>> GetDebts();
    Task<Invoice?> GetInvoiceById(int invoiceNumber);
}

}