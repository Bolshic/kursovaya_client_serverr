using Microsoft.AspNetCore.Mvc;
using DormitoryAPI.Data.Repository;
using DormitoryAPI.Models;
using DormitoryAPI.Models.ViewModels;

namespace DormitoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentsController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        // GET: api/Students?faculty=...&course=...&group=...&search=...
        // Возвращает студентов с комнатой и корпусом (для вкладки "Студенты и фильтры")
        [HttpGet]
        public async Task<IActionResult> GetStudents(
            [FromQuery] string? faculty,
            [FromQuery] int? course,
            [FromQuery] string? group,
            [FromQuery] string? search)
        {
            var students = await _studentRepository.GetAllWithFiltersAsync(faculty, course, group, search);
            return Ok(students);
        }

        // GET: api/Students/current-residents
        // Используется в отчёте "Проживающие"
        [HttpGet("current-residents")]
        public async Task<IActionResult> GetCurrentResidents()
        {
            var residents = await _studentRepository.GetCurrentResidents();
            return Ok(residents);
        }

        // GET: api/Students/faculty/{facultyName}
        // Если нужно – можно оставить для совместимости
        [HttpGet("faculty/{facultyName}")]
        public async Task<IActionResult> GetStudentsByFaculty(string facultyName)
        {
            var students = await _studentRepository.GetCurrentResidenceByFaculty(facultyName);
            return Ok(students);
        }

        // GET: api/Students/{passportNum}
        // Поиск студента по паспорту (для вкладки "Действия")
        [HttpGet("{passportNum}")]
        public async Task<IActionResult> GetStudentByPassport(string passportNum)
        {
            var student = await _studentRepository.GetByPassportAsync(passportNum);
            if (student == null)
                return NotFound(new { message = "Студент не найден" });
            return Ok(student);
        }

        // PUT: api/Students/{passportNum}
        // Обновление ФИО и/или группы
        [HttpPut("{passportNum}")]
        public async Task<IActionResult> UpdateStudent(string passportNum, [FromBody] UpdateStudentRequest request)
        {
            if (passportNum != request.PassportNum)
                return BadRequest(new { message = "Паспорт в URL и в теле запроса не совпадают" });

            var existing = await _studentRepository.GetByPassportAsync(passportNum);
            if (existing == null)
                return NotFound(new { message = "Студент не найден" });

            // Обновляем только переданные поля
            var updatedStudent = new Student
            {
                PassportNum = passportNum,
                Fio = string.IsNullOrWhiteSpace(request.Fio) ? existing.Fio : request.Fio,
                GroupName = string.IsNullOrWhiteSpace(request.GroupName) ? existing.GroupName : request.GroupName
            };

            await _studentRepository.UpdateAsync(updatedStudent);
            return Ok(new { message = "Данные обновлены" });
        }

        // POST: api/Students (если нужно добавление нового студента)
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] Student student)
        {
            if (student == null || string.IsNullOrWhiteSpace(student.PassportNum))
                return BadRequest(new { message = "Некорректные данные" });

            var existing = await _studentRepository.GetByPassportAsync(student.PassportNum);
            if (existing != null)
                return Conflict(new { message = "Студент с таким паспортом уже существует" });

            await _studentRepository.AddAsync(student);
            return CreatedAtAction(nameof(GetStudentByPassport), new { passportNum = student.PassportNum }, student);
        }

        // DELETE: api/Students/{passportNum} (если нужно)
        [HttpDelete("{passportNum}")]
        public async Task<IActionResult> DeleteStudent(string passportNum)
        {
            var existing = await _studentRepository.GetByPassportAsync(passportNum);
            if (existing == null)
                return NotFound(new { message = "Студент не найден" });

            await _studentRepository.DeleteAsync(passportNum);
            return Ok(new { message = "Студент удалён" });
        }
    }

    // Вспомогательный DTO для обновления
    public class UpdateStudentRequest
    {
        public string PassportNum { get; set; } = "";
        public string? Fio { get; set; }
        public string? GroupName { get; set; }
    }
}