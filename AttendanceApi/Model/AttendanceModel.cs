using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceApi.Model
{
    public class AttendanceModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("StudentModel")]
        public int StudentId { get; set; }

        public StudentModel? Student { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Present|Absent)$", ErrorMessage = "Status must be either Present or Absent")]
        public string Status { get; set; } = "Absent";
        public int? MarkedByAdminId { get; set; }
    }
}

