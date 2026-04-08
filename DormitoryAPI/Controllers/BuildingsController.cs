using Microsoft.AspNetCore.Mvc;
using DormitoryAPI.Models;
using DormitoryAPI.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DormitoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuildingsController : ControllerBase
    {
        private readonly IBuildingRepository _buildingRepo;

        public BuildingsController(IBuildingRepository buildingRepo)
        {
            _buildingRepo = buildingRepo;
        }

        // GET: api/buildings
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var buildings = await _buildingRepo.GetAllAsync();
            return Ok(buildings);
        }

        // GET: api/buildings/{buildingName}
        [HttpGet("{buildingName}")]
        public async Task<IActionResult> GetByName(string buildingName)
        {
            var building = await _buildingRepo.GetByNameAsync(buildingName);
            if (building == null)
                return NotFound();
            return Ok(building);
        }

        // POST: api/buildings
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Building building)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _buildingRepo.AddAsync(building);
            return CreatedAtAction(nameof(GetByName), new { buildingName = building.BuildingName }, building);
        }

        // PUT: api/buildings/{buildingName}
        [HttpPut("{buildingName}")]
        public async Task<IActionResult> Update(string buildingName, [FromBody] Building building)
        {
            if (buildingName != building.BuildingName)
                return BadRequest("Имя здания в пути не совпадает с телом");

            var existing = await _buildingRepo.GetByNameAsync(buildingName);
            if (existing == null)
                return NotFound();

            await _buildingRepo.UpdateAsync(building);
            return NoContent();
        }

        // DELETE: api/buildings/{buildingName}
        [HttpDelete("{buildingName}")]
        public async Task<IActionResult> Delete(string buildingName)
        {
            var existing = await _buildingRepo.GetByNameAsync(buildingName);
            if (existing == null)
                return NotFound();

            await _buildingRepo.DeleteAsync(buildingName);
            return NoContent();
        }
    }
}