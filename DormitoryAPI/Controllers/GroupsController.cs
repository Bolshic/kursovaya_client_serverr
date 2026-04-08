using Microsoft.AspNetCore.Mvc;
using DormitoryAPI.Models;
using DormitoryAPI.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IGroupRepository _groupRepo;
    public GroupsController(IGroupRepository groupRepo) => _groupRepo = groupRepo;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? faculty) =>
        Ok(string.IsNullOrEmpty(faculty) ? await _groupRepo.GetAllAsync() : await _groupRepo.GetByFacultyAsync(faculty));

    [HttpGet("{groupName}")]
    public async Task<IActionResult> GetByName(string groupName)
    {
        var group = await _groupRepo.GetByNameAsync(groupName);
        if (group == null) return NotFound();
        return Ok(group);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Group group)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _groupRepo.AddAsync(group);
        return CreatedAtAction(nameof(GetByName), new { groupName = group.GroupName }, group);
    }

    [HttpPut("{groupName}")]
    public async Task<IActionResult> Update(string groupName, [FromBody] Group group)
    {
        if (groupName != group.GroupName) return BadRequest();
        var existing = await _groupRepo.GetByNameAsync(groupName);
        if (existing == null) return NotFound();
        await _groupRepo.UpdateAsync(group);
        return NoContent();
    }

    [HttpDelete("{groupName}")]
    public async Task<IActionResult> Delete(string groupName)
    {
        var existing = await _groupRepo.GetByNameAsync(groupName);
        if (existing == null) return NotFound();
        await _groupRepo.DeleteAsync(groupName);
        return NoContent();
    }
}