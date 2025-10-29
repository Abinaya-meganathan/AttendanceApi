using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceApi.Model
{
    public class StudentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [NotMapped]
        [Required(ErrorMessage = "First Name is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Name must contain only alphabets")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string? First_Name { get; set; }


        [NotMapped]
        [Required(ErrorMessage = "Last Name is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Name must contain only alphabets")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string? Last_Name { get; set; }


        [Required(ErrorMessage = "Full Name is required")]
        [RegularExpression("^[A-Za-z]+$", ErrorMessage = "Name must contain only alphabets")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string? Full_Name { get; set; }


        [Required(ErrorMessage = "Phone Number is required")]
        [Range(1000000000, 9999999999, ErrorMessage = "Phone number must be a 10-digit number")]
        public long Phone_Number { get; set; }


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }


        [NotMapped]
        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string? Confirm_Password { get; set; }

        //role claim
        public string Role { get; set; } = "Student";

        //otp verification

        public string? EmailOtp { get; set; }
        public DateTime? EmailOtpExpiry { get; set; }
        public bool IsEmailVerified { get; set; } = false;

    }
}
