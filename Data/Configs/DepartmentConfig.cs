using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace studentsapi.Data.Configs
{
    public class DepartmentConfig : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.DepartmentName)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(s => s.Description)
                .IsRequired(false);
                
           
            builder.HasData(new List<Department>()
                {
                new() {

                Id = 1,
                DepartmentName = "Computer Science",
                Description = "Department of Computer Science"
                },
                new() {
                    Id = 2,
                    DepartmentName = "Mathematics",
                    Description = "Department of Mathematics"
                }
            });
        }
    }
}
