using Microsoft.AspNetCore.Mvc;
using DormitoryAPI.Models;
using DormitoryAPI.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IReportRepository _reportRepo;
    public ReportsController(IReportRepository reportRepo) => _reportRepo = reportRepo;

    [HttpGet("free-rooms")]
    public async Task<IActionResult> GetFreeRooms() => Ok(await _reportRepo.GetFreeRooms());


    [HttpGet("debts")]
    public async Task<IActionResult> GetDebts() => Ok(await _reportRepo.GetDebts());


    [HttpGet("occupancy")]
    public async Task<IActionResult> GetOccupancy() => Ok(await _reportRepo.GetOccupancyStats());
    
    [HttpGet("residents/faculty/{facultyName}")]
    public async Task<IActionResult> ResidentsByFaculty(string facultyName) =>
        Ok(await _reportRepo.GetResidentsByFaculty(facultyName));

    [HttpGet("payments/monthly/{year}")]
    public async Task<IActionResult> MonthlyPayments(int year) =>
        Ok(await _reportRepo.GetMonthlyPayments(year));
}