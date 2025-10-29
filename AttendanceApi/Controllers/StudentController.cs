using AttendanceApi.Model;
using AttendanceApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IRepository _repository;
        public StudentController(IRepository repository)
        {
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpGet("attendance/{id}")]
        public async Task<IActionResult> GetMyAttendance(int id)
        {
           var attendance = await  _repository.GetStudentByIdAsync(id);
           
           return Ok();
        }

    }
}
