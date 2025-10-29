using AttendanceApi.Model;
using AttendanceApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IRepository _repo;
        public AttendanceController(IRepository repo)
        {
            _repo = repo;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("mark")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> MarkAttendance([FromBody] AttendanceModel attendance)
        {
            // ✅ Extract the Admin ID from JWT token
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int adminId))
                return Unauthorized(new { message = "Invalid admin ID in token" });

            // Check if student exists
            var student = await _repo.GetStudentByIdAsync(attendance.StudentId);
            if (student == null)
                return NotFound(new { message = "Student not found" });

            // Create a new attendance record with Admin ID
            var newAttendance = new AttendanceModel
            {
                StudentId = attendance.StudentId,
                Status = attendance.Status,
                Date = DateTime.UtcNow.Date,
                MarkedByAdminId = adminId // ✅ Automatically use logged-in admin's ID
            };

            await _repo.AddAttendanceAsync(newAttendance);

            return Ok(new { message = "Attendance marked successfully", markedByAdminId = adminId });
        }

        [Authorize(Roles = "Student,Admin,SuperAdmin")]
        [HttpGet("month/{studentId}/{year}/{month}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMonthly(int studentId, int year, int month)
        {
            // If Student → they can only access their own records
            if (User.IsInRole("Student"))
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdStr, out var loggedInStudentId) || loggedInStudentId != studentId)
                {
                    return Forbid(); // Student trying to access another student's data
                }
            }

            // If Admin or SuperAdmin → allow all students' data
            var data = await _repo.GetAttendanceByMonthAsync(studentId, year, month);

            if (data == null || !data.Any())
                return NotFound("No attendance found for the given student and month.");

            return Ok(data);
        }

        // ✅ NEW: Get attendance with admin details (for reporting)
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpGet("details/{studentId}/{year}/{month}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMonthlyWithAdminDetails(int studentId, int year, int month)
        {
            var data = await _repo.GetAttendanceWithAdminDetailsAsync(studentId, year, month);

            if (data == null || !data.Any())
                return NotFound("No attendance found for the given student and month.");

            return Ok(data);
        }
    }
}