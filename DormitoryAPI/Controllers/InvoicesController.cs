using Microsoft.AspNetCore.Mvc;
using DormitoryAPI.Models;
using DormitoryAPI.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceRepository _invoiceRepo;
    public InvoicesController(IInvoiceRepository invoiceRepo) => _invoiceRepo = invoiceRepo;

    [HttpPost("generate-monthly")]
    public async Task<IActionResult> GenerateMonthly([FromBody] MonthlyRequest request)
    {
        await _invoiceRepo.GenerateMonthlyInvoices(request.Year, request.Month);
        return Ok();
    }

    [HttpPost("laundry")]
    public async Task<IActionResult> CreateLaundry([FromBody] LaundryRequest request)
    {
        await _invoiceRepo.CreateLaundryInvoice(request.OrderNumber, request.Quantity, request.ServiceDate);
        return Ok();
    }

    [HttpPost("payment")]
    public async Task<IActionResult> MakePayment([FromBody] PaymentRequest request)
    {
        await _invoiceRepo.MakePayment(request.InvoiceNumber, request.Amount, request.PayDate);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetInvoices([FromQuery] string? passportNum, [FromQuery] string? status) =>
        Ok(await _invoiceRepo.GetInvoices(passportNum, status));

    [HttpGet("debts")]
    public async Task<IActionResult> GetDebts() => Ok(await _invoiceRepo.GetDebts());

    [HttpGet("{invoiceNumber}")]
    public async Task<IActionResult> GetById(int invoiceNumber)
    {
        var invoice = await _invoiceRepo.GetInvoiceById(invoiceNumber);
        if (invoice == null) return NotFound();
        return Ok(invoice);
    }
}

public class MonthlyRequest
{
    public int Year { get; set; }
    public int Month { get; set; }
}

public class LaundryRequest
{
    public int OrderNumber { get; set; }
    public int Quantity { get; set; } = 1;
    public DateTime ServiceDate { get; set; } = DateTime.Today;
}

public class PaymentRequest
{
    public int InvoiceNumber { get; set; }
    public decimal Amount { get; set; }
    public DateTime PayDate { get; set; } = DateTime.Today;
}