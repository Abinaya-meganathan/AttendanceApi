using AttendanceApi.Model;
using AttendanceApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AttendanceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepository _repo;
        private readonly IConfiguration _config;
        private readonly EmailService _email;

        public AuthController(IRepository repo, IConfiguration config, EmailService email)
        {
            _repo = repo;
            _config = config;
            _email = email;
        }

        // ✅ LOGIN
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var (userId, role, fullName) = await _repo.AuthenticateAsync(login.Email, login.Password);

            if (userId == 0)
                return Unauthorized(new { message = "Invalid credentials or not approved." });

            var token = GenerateJwtToken(userId, role, fullName);
            return Ok(new { token, role, name = fullName });
        }

        // ✅ JWT Generator
        private string GenerateJwtToken(int userId, string role, string fullName)
        {
            var jwtSection = _config.GetSection("JwtSettings");
            var secret = jwtSection["SecretKey"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expiry = int.Parse(jwtSection["ExpiryMinutes"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, fullName),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(expiry),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ✅ OTP Request - FIXED to accept object
        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] OtpEmailRequest req)
        {
            if (string.IsNullOrEmpty(req.Email))
                return BadRequest(new { message = "Email is required" });

            var otp = new Random().Next(100000, 999999).ToString();
            var expiry = DateTime.UtcNow.AddMinutes(10);

            await _repo.SetEmailOtpAsync(req.Email, otp, expiry);
            await _email.SendEmailAsync(req.Email, "Your Attendance OTP", $"Your OTP is <b>{otp}</b>. It expires in 10 minutes.");

            return Ok(new { message = "OTP sent successfully (if email exists)." });
        }

        // ✅ OTP Verify
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpRequest req)
        {
            var result = await _repo.VerifyEmailOtpAsync(req.Email, req.Otp);

            if (!result)
                return BadRequest(new { message = "Invalid or expired OTP." });

            return Ok(new { message = "Email verified successfully." });
        }

        public class OtpRequest
        {
            public string Email { get; set; }
            public string Otp { get; set; }
        }

        public class OtpEmailRequest
        {
            public string Email { get; set; }
        }

        // ✅ REGISTER ADMIN
        [AllowAnonymous]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] AdminModel admin)
        {
            Console.WriteLine($"[API] RegisterAdmin called for: {admin.Email}");

            // Remove NotMapped fields from validation
            ModelState.Remove("First_Name");
            ModelState.Remove("Last_Name");
            ModelState.Remove("Confirm_Password");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine($"[API] Validation failed: {string.Join(", ", errors)}");
                return BadRequest(new { message = "Validation failed", errors });
            }

            var result = await _repo.RegisterAdminAsync(admin);
            if (!result)
            {
                Console.WriteLine($"[API] Admin email already exists: {admin.Email}");
                return BadRequest(new { message = "Failed to register admin. Email may already exist." });
            }

            Console.WriteLine($"[API] Admin registered successfully: {admin.Email}");
            return Ok(new { message = "Admin registered successfully. Awaiting SuperAdmin approval." });
        }

        // ✅ REGISTER STUDENT
        [AllowAnonymous]
        [HttpPost("register-student")]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentModel student)
        {
            Console.WriteLine($"[API] RegisterStudent called for: {student.Email}");

            // Remove NotMapped fields from validation
            ModelState.Remove("First_Name");
            ModelState.Remove("Last_Name");
            ModelState.Remove("Confirm_Password");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine($"[API] Validation failed: {string.Join(", ", errors)}");
                return BadRequest(new { message = "Validation failed", errors });
            }

            var result = await _repo.RegisterStudentAsync(student);
            if (!result)
            {
                Console.WriteLine($"[API] Student email already exists: {student.Email}");
                return BadRequest(new { message = "Failed to register student. Email may already exist." });
            }

            Console.WriteLine($"[API] Student registered successfully: {student.Email}");
            return Ok(new { message = "Student registered successfully." });
        }
    }
}