using AttendanceApi.DTO;
using AttendanceApi.Model;
using AttendanceApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using static Org.BouncyCastle.Math.EC.ECCurve;


namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AdminController : ControllerBase
    {

        private readonly IRepository _repository;
        private readonly IConfiguration _config;
        public AdminController(IRepository repository, IConfiguration config)
        {
            _repository = repository;
            _config = config;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("students")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _repository.GetAllStudentsAsync();
            return Ok(students);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("student/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] UpdateStudentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = await _repository.GetStudentByIdAsync(id);
            if (existing == null)
                return NotFound();

            existing.First_Name = dto.First_Name;
            existing.Last_Name = dto.Last_Name;
            existing.Email = dto.Email;
            existing.Phone_Number = dto.Phone_Number;

            await _repository.UpdateStudentAsync(existing);
            return Ok(new { message = "Student updated successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("student/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            await _repository.DeleteStudentAsync(id);
            return Ok(new { message = "Student deleted successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("attendance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAttendance([FromBody] AttendanceModel attendance)
        {
            await _repository.AddAttendanceAsync(attendance);
            return Ok(new { message = "Attendance marked successfully" });
        }

        // ✅ Get Monthly Attendance
        [Authorize(Roles = "Student,Admin,SuperAdmin")]
        [HttpGet("attendance/month/{studentId}/{year}/{month}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public async Task<IActionResult> GetMonthlyAttendance(int studentId, int year, int month)
        {
            var data = await _repository.GetAttendanceByMonthAsync(studentId, year, month);
            return Ok(data);
        }


    }

}
