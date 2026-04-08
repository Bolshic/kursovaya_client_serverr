using Microsoft.AspNetCore.Mvc;
using DormitoryAPI.Models;
using DormitoryAPI.Data.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DormitoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacultiesController : ControllerBase
    {
        private readonly IFacultyRepository _facultyRepo;

        public FacultiesController(IFacultyRepository facultyRepo)
        {
            _facultyRepo = facultyRepo;
        }

        // GET: api/faculties
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var faculties = await _facultyRepo.GetAllAsync();
            return Ok(faculties);
        }

        // GET: api/faculties/{facultyName}
        [HttpGet("{facultyName}")]
        public async Task<IActionResult> GetByName(string facultyName)
        {
            var faculty = await _facultyRepo.GetByNameAsync(facultyName);
            if (faculty == null)
                return NotFound();
            return Ok(faculty);
        }

        // POST: api/faculties
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Faculty faculty)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _facultyRepo.AddAsync(faculty);
            return CreatedAtAction(nameof(GetByName), new { facultyName = faculty.FacultyName }, faculty);
        }

        // PUT: api/faculties/{facultyName}
        [HttpPut("{facultyName}")]
        public async Task<IActionResult> Update(string facultyName, [FromBody] Faculty faculty)
        {
            if (facultyName != faculty.FacultyName)
                return BadRequest("Имя факультета в пути не совпадает с телом");

            var existing = await _facultyRepo.GetByNameAsync(facultyName);
            if (existing == null)
                return NotFound();

            await _facultyRepo.UpdateAsync(faculty);
            return NoContent();
        }

        // DELETE: api/faculties/{facultyName}
        [HttpDelete("{facultyName}")]
        public async Task<IActionResult> Delete(string facultyName)
        {
            var existing = await _facultyRepo.GetByNameAsync(facultyName);
            if (existing == null)
                return NotFound();

            await _facultyRepo.DeleteAsync(facultyName);
            return NoContent();
        }
    }
}