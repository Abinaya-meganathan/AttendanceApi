using AttendanceApi.Data;
using AttendanceApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Repository
{
    public class Repository : IRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly EmailService _email;

        public Repository(ApplicationDbContext context, EmailService email)
        {
            _context = context;
            _email = email;
        }

        // Student
        public async Task<IEnumerable<StudentModel>> GetAllStudentsAsync()
            => await _context.StudentTable.ToListAsync();

        public async Task<StudentModel?> GetStudentByIdAsync(int id)
            => await _context.StudentTable.FindAsync(id);

        public async Task UpdateStudentAsync(StudentModel student)
        {
            _context.StudentTable.Update(student);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _context.StudentTable.FindAsync(id);
            if (student != null)
            {
                _context.StudentTable.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

        // Admin
        public async Task<AdminModel?> GetAdminByEmailAsync(string email)
            => await _context.AdminTable.FirstOrDefaultAsync(a => a.Email == email);

        public async Task<AdminModel?> GetAdminByIdAsync(int id)
            => await _context.AdminTable.FindAsync(id);

        // Attendance
        public async Task AddAttendanceAsync(AttendanceModel attendance)
        {
            _context.AttendanceTable.Add(attendance);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AttendanceModel>> GetAttendanceByMonthAsync(int studentId, int year, int month)
        {
            return await _context.AttendanceTable
                .Where(a => a.StudentId == studentId &&
                            a.Date.Year == year &&
                            a.Date.Month == month)
                .OrderBy(a => a.Date)
                .ToListAsync();
        }

        // ✅ NEW: Get attendance with admin details
        public async Task<IEnumerable<object>> GetAttendanceWithAdminDetailsAsync(int studentId, int year, int month)
        {
            var result = await _context.AttendanceTable
                .Where(a => a.StudentId == studentId &&
                            a.Date.Year == year &&
                            a.Date.Month == month)
                .Join(_context.AdminTable,
                      attendance => attendance.MarkedByAdminId,
                      admin => admin.Id,
                      (attendance, admin) => new
                      {
                          attendance.Id,
                          attendance.StudentId,
                          attendance.Date,
                          attendance.Status,
                          attendance.MarkedByAdminId,
                          AdminName = admin.Full_Name,
                          AdminEmail = admin.Email
                      })
                .OrderBy(a => a.Date)
                .ToListAsync();

            return result;
        }

        // Authentication
        public async Task<(int userId, string role, string fullName)> AuthenticateAsync(string email, string password)
        {
            // Check Student
            var student = await _context.StudentTable
                .FirstOrDefaultAsync(s => s.Email == email && s.Password == password && s.IsEmailVerified);

            if (student != null)
                return (student.Id, "Student", student.Full_Name);

            // Check Admin
            var admin = await _context.AdminTable
                .FirstOrDefaultAsync(a => a.Email == email && a.Password == password && a.IsEmailVerified);

            if (admin != null && string.Equals(admin.Status, "Approved", StringComparison.OrdinalIgnoreCase))
                return (admin.Id, "Admin", admin.Full_Name);

            // Check SuperAdmin
            var superAdmin = await _context.AdminTable
                .FirstOrDefaultAsync(a => a.Email == "superadmin@gmail.com" && a.Password == "superadmin");

            if (superAdmin != null)
                return (superAdmin.Id, "SuperAdmin", superAdmin.Full_Name);

            return (0, null, null);
        }

        // SuperAdmin
        public async Task<IEnumerable<AdminModel>> GetPendingAdminsAsync()
        {
            return await _context.AdminTable
                .Where(a => a.Status == "Pending")
                .ToListAsync();
        }

        public async Task ApproveAdminAsync(int adminId)
        {
            var admin = await _context.AdminTable.FindAsync(adminId);
            if (admin == null) return;
            admin.Status = "Approved";
            await _context.SaveChangesAsync();
        }

        public async Task DeclineAdminAsync(int adminId)
        {
            var admin = await _context.AdminTable.FindAsync(adminId);
            if (admin == null) return;
            admin.Status = "Declined";
            await _context.SaveChangesAsync();
        }

        public async Task EnsureSuperAdminExistsAsync()
        {
            var super = await _context.AdminTable.FirstOrDefaultAsync(a => a.Email == "superadmin@gmail.com");
            if (super == null)
            {
                var s = new AdminModel
                {
                    Full_Name = "Super Admin",
                    Email = "superadmin@gmail.com",
                    Password = "superadmin",
                    Phone_Number = 9999999999,
                    Status = "Approved",
                    IsEmailVerified = true,
                    Role = "SuperAdmin"
                };
                _context.AdminTable.Add(s);
                await _context.SaveChangesAsync();
            }
        }

        // OTP
        public async Task SetEmailOtpAsync(string email, string otp, DateTime expiry)
        {
            var student = await _context.StudentTable.FirstOrDefaultAsync(s => s.Email == email);
            if (student != null)
            {
                student.EmailOtp = otp;
                student.EmailOtpExpiry = expiry;
                await _context.SaveChangesAsync();
                return;
            }

            var admin = await _context.AdminTable.FirstOrDefaultAsync(a => a.Email == email);
            if (admin != null)
            {
                admin.Otp = otp;
                admin.EmailOtpExpiry = expiry;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> VerifyEmailOtpAsync(string email, string otp)
        {
            var student = await _context.StudentTable.FirstOrDefaultAsync(s => s.Email == email);
            if (student != null)
            {
                if (student.EmailOtp == otp && student.EmailOtpExpiry > DateTime.UtcNow)
                {
                    student.IsEmailVerified = true;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }

            var admin = await _context.AdminTable.FirstOrDefaultAsync(a => a.Email == email);
            if (admin != null)
            {
                if (admin.Otp == otp && admin.EmailOtpExpiry > DateTime.UtcNow)
                {
                    
                    admin.IsEmailVerified = true;
                    await _context.SaveChangesAsync();
                    return true;
                }
                return false;
            }

            return false;
        }

        // Registration
        public async Task<bool> RegisterAdminAsync(AdminModel admin)
        {
            Console.WriteLine($"[REPO] Checking if admin exists: {admin.Email}");

            // Check if email already exists (case-insensitive)
            var exists = await _context.AdminTable
                .AnyAsync(a => a.Email.ToLower() == admin.Email.ToLower());

            if (exists)
            {
                Console.WriteLine($"[REPO] Admin already exists: {admin.Email}");
                return false;
            }

            Console.WriteLine($"[REPO] Adding new admin: {admin.Email}");
            _context.AdminTable.Add(admin);
            await _context.SaveChangesAsync();

            Console.WriteLine($"[REPO] Admin saved with ID: {admin.Id}");
            return true;
        }

        public async Task<bool> RegisterStudentAsync(StudentModel student)
        {
            Console.WriteLine($"[REPO] Checking if student exists: {student.Email}");

            // Check if email already exists (case-insensitive)
            var exists = await _context.StudentTable
                .AnyAsync(s => s.Email.ToLower() == student.Email.ToLower());

            if (exists)
            {
                Console.WriteLine($"[REPO] Student already exists: {student.Email}");
                return false;
            }

            Console.WriteLine($"[REPO] Adding new student: {student.Email}");
            _context.StudentTable.Add(student);
            await _context.SaveChangesAsync();

            Console.WriteLine($"[REPO] Student saved with ID: {student.Id}");
            return true;
        }
    }
}