using System.ComponentModel.DataAnnotations;

namespace AttendanceApi.DTO
{
    public class UpdateStudentDto
    {
        public int Id { get; set; }
        [Required] public string? First_Name { get; set; }
        [Required] public string? Last_Name { get; set; }
        [Required, EmailAddress] public string? Email { get; set; }
        public long Phone_Number { get; set; }
    }
}
