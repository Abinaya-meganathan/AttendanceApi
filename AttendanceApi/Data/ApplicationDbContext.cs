using AttendanceApi.Model;
using Microsoft.EntityFrameworkCore;

namespace AttendanceApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<StudentModel> StudentTable { get; set; }
        public DbSet<AdminModel> AdminTable { get; set; }
        public DbSet<AttendanceModel> AttendanceTable { get; set; }

    }
}
