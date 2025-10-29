using AttendanceApi.Model;
using System.ComponentModel.DataAnnotations;

namespace AttendanceApi.DTO
{
    public class StudentRegistrationDto
    {
        [Required]
        public StudentModel Student { get; set; }

        public string Otp { get; set; }

    }
}
