using System.ComponentModel.DataAnnotations;

namespace AttendanceApi.Model
{
    public class LoginModel
    {
        [Required (ErrorMessage = "data is required")]
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
