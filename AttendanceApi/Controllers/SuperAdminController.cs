using AttendanceApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "SuperAdmin")]
    public class SuperAdminController : Controller
    {
        private readonly IRepository _repo;
        public SuperAdminController(IRepository repo) { _repo = repo; }

        [HttpGet("pending-admins")]
        public async Task<IActionResult> GetPendingAdmins()
        {
            var list = await _repo.GetPendingAdminsAsync();
            return Ok(list);
        }

        [HttpPost("approve-admin/{id}")]
        public async Task<IActionResult> ApproveAdmin(int id)
        {
            await _repo.ApproveAdminAsync(id);
            return Ok(new { message = "Approved" });
        }

        [HttpPost("decline-admin/{id}")]
        public async Task<IActionResult> DeclineAdmin(int id)
        {
            await _repo.DeclineAdminAsync(id);
            return Ok(new { message = "Declined" });
        }
    }
}
