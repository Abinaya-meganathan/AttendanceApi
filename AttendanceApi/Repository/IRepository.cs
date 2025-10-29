using AttendanceApi.Model;

namespace AttendanceApi.Repository
{
    public interface IRepository
    {
        // Student
        Task<IEnumerable<StudentModel>> GetAllStudentsAsync();
        Task<StudentModel?> GetStudentByIdAsync(int id);
        Task UpdateStudentAsync(StudentModel student);
        Task DeleteStudentAsync(int id);

        // Admin
        Task<AdminModel?> GetAdminByEmailAsync(string email);
        Task<AdminModel?> GetAdminByIdAsync(int id);

        // Attendance
        Task AddAttendanceAsync(AttendanceModel attendance);
        Task<IEnumerable<AttendanceModel>> GetAttendanceByMonthAsync(int studentId, int year, int month);

        // ✅ NEW: Get attendance with admin details
        Task<IEnumerable<object>> GetAttendanceWithAdminDetailsAsync(int studentId, int year, int month);

        // Login check (for both)
        Task<(int userId, string role, string fullName)> AuthenticateAsync(string email, string password);

        // SuperAdmin
        Task<IEnumerable<AdminModel>> GetPendingAdminsAsync();
        Task ApproveAdminAsync(int adminId);
        Task DeclineAdminAsync(int adminId);
        Task EnsureSuperAdminExistsAsync();

        // OTP
        Task SetEmailOtpAsync(string email, string otp, DateTime expiry);
        Task<bool> VerifyEmailOtpAsync(string email, string otp);

        // Registration
        Task<bool> RegisterAdminAsync(AdminModel admin);
        Task<bool> RegisterStudentAsync(StudentModel student);
    }
}