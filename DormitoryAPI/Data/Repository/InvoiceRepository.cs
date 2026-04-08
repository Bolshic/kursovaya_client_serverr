using Dapper;
using DormitoryAPI.Models;
using DormitoryAPI.Models.ViewModels;

namespace DormitoryAPI.Data.Repository
{
    public class InvoiceRepository : IInvoiceRepository
{
    private readonly DapperContext _context;
    public InvoiceRepository(DapperContext context) => _context = context;

    public async Task<int> GenerateMonthlyInvoices(int year, int month)
    {
        var sql = "CALL generate_monthly_invoices(@p_year, @p_month)";
        using var connection = _context.CreateConnection();
        return await connection.ExecuteAsync(sql, new { p_year = year, p_month = month });
    }

    public async Task<int> CreateLaundryInvoice(int orderNumber, int quantity, DateTime serviceDate)
    {
        var sql = "CALL create_laundry_invoice(@p_order_number, @p_quantity, @p_service_date)";
        using var connection = _context.CreateConnection();
        return await connection.ExecuteAsync(sql, new
        {
            p_order_number = orderNumber,
            p_quantity = quantity,
            p_service_date = serviceDate.Date
        });
    }

    public async Task<int> MakePayment(int invoiceNumber, decimal amount, DateTime payDate)
    {
        var sql = "CALL make_payment(@p_invoice_number, @p_amount, @p_pay_date)";
        using var connection = _context.CreateConnection();
        return await connection.ExecuteAsync(sql, new
        {
            p_invoice_number = invoiceNumber,
            p_amount = amount,
            p_pay_date = payDate.Date
        });
    }

    public async Task<IEnumerable<InvoiceDetailsView>> GetInvoices(string? passportNum = null, string? status = null)
    {
        var sql = "SELECT * FROM v_invoice_details WHERE 1=1";
        var parameters = new DynamicParameters();
        if (!string.IsNullOrEmpty(passportNum))
        {
            sql += " AND passport_num = @passportNum";
            parameters.Add("@passportNum", passportNum);
        }
        if (!string.IsNullOrEmpty(status))
        {
            sql += " AND status = @status";
            parameters.Add("@status", status);
        }
        sql += " ORDER BY period_start DESC";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<InvoiceDetailsView>(sql, parameters);
    }

    public async Task<IEnumerable<DebtView>> GetDebts()
    {
        var sql = "SELECT * FROM v_debts ORDER BY total_debt DESC";
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<DebtView>(sql);
    }

    public async Task<Invoice?> GetInvoiceById(int invoiceNumber)
    {
        var sql = "SELECT invoice_number, order_number, service_type, period_start, period_end, amount, paid_amount, status, invoice_date FROM Invoice WHERE invoice_number = @invoiceNumber";
        using var connection = _context.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Invoice>(sql, new { invoiceNumber });
    }
}
}