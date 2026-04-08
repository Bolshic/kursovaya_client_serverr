using Microsoft.AspNetCore.Mvc;
using DormitoryAPI.Models;
using DormitoryAPI.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class ResidenceController : ControllerBase
{
    private readonly IResidenceRepository _residenceRepo;
    public ResidenceController(IResidenceRepository residenceRepo) => _residenceRepo = residenceRepo;

    [HttpPost("settle")]
    public async Task<IActionResult> Settle([FromBody] SettleRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _residenceRepo.SettleStudent(request.PassportNum, request.RoomId, request.DateIn);
        return Ok();
    }

    [HttpPost("evict")]
    public async Task<IActionResult> Evict([FromBody] EvictRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _residenceRepo.EvictStudent(request.OrderNumber, request.DateOut);
        return Ok();
    }

    [HttpPost("relocate")]
    public async Task<IActionResult> Relocate([FromBody] RelocateRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _residenceRepo.RelocateStudent(request.OrderNumber, request.NewRoomId, request.RelocationDate);
        return Ok();
    }

    [HttpGet("history/student/{passportNum}")]
    public async Task<IActionResult> HistoryByStudent(string passportNum) =>
        Ok(await _residenceRepo.GetHistoryByStudent(passportNum));

    [HttpGet("history/room/{roomId}")]
    public async Task<IActionResult> HistoryByRoom(int roomId) =>
        Ok(await _residenceRepo.GetHistoryByRoom(roomId));

    [HttpGet("current/{passportNum}")]
    public async Task<IActionResult> CurrentResidence(string passportNum)
    {
        var res = await _residenceRepo.GetCurrentResidence(passportNum);
        if (res == null) return NotFound();
        return Ok(res);
    }
}

// DTOs для запросов
public class SettleRequest
{
    public string PassportNum { get; set; } = null!;
    public int RoomId { get; set; }
    public DateTime DateIn { get; set; } = DateTime.Today;
}

public class EvictRequest
{
    public int OrderNumber { get; set; }
    public DateTime DateOut { get; set; } = DateTime.Today;
}

public class RelocateRequest
{
    public int OrderNumber { get; set; }
    public int NewRoomId { get; set; }
    public DateTime RelocationDate { get; set; } = DateTime.Today;
}