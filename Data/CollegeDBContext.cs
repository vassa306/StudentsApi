using Microsoft.EntityFrameworkCore;

namespace studentsapi.Data
{
    public class CollegeDBContext : DbContext
    {
        public CollegeDBContext(DbContextOptions<CollegeDBContext> options) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Department> Departments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configs.StudentConfig());
            modelBuilder.ApplyConfiguration(new Configs.DepartmentConfig());
        }
    }   
}
