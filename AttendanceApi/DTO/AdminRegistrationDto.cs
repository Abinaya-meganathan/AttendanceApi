using AttendanceApi.Model;
using System.ComponentModel.DataAnnotations;

namespace AttendanceApi.DTO
{
    public class AdminRegistrationDto
    {
        [Required]
        public AdminModel Admin { get; set; }

        public string Otp { get; set; }

    }
}
