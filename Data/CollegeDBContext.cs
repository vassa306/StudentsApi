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
        public DbSet<User> Users { get; set; }
        public DbSet<Role> UserRoles { get; set; }

        public DbSet<RolePrivilege> RolesPrivileges { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new Configs.StudentConfig());
            modelBuilder.ApplyConfiguration(new Configs.DepartmentConfig());
            modelBuilder.ApplyConfiguration(new Configs.UserConfig());
            modelBuilder.ApplyConfiguration(new Configs.UserRoleConfig());
            modelBuilder.ApplyConfiguration(new Configs.RolePrivilegeConfig());
        }
    }   
}
