using Microsoft.AspNetCore.Mvc;
using DormitoryAPI.Models;
using DormitoryAPI.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomRepository _roomRepo;
    public RoomsController(IRoomRepository roomRepo) => _roomRepo = roomRepo;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? building) =>
        Ok(string.IsNullOrEmpty(building) ? await _roomRepo.GetAllAsync() : await _roomRepo.GetByBuildingAsync(building));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var room = await _roomRepo.GetByIdAsync(id);
        if (room == null) return NotFound();
        return Ok(room);
    }

    [HttpGet("free")]
    public async Task<IActionResult> GetFreeRooms() => Ok(await _roomRepo.GetFreeRoomsAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Room room)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _roomRepo.AddAsync(room);
        return CreatedAtAction(nameof(GetById), new { id = room.RoomId }, room);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Room room)
    {
        if (id != room.RoomId) return BadRequest();
        var existing = await _roomRepo.GetByIdAsync(id);
        if (existing == null) return NotFound();
        await _roomRepo.UpdateAsync(room);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _roomRepo.GetByIdAsync(id);
        if (existing == null) return NotFound();
        await _roomRepo.DeleteAsync(id);
        return NoContent();
    }
}