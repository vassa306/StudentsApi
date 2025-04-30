using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace studentsapi.Data.Configs
{
    public class StudentConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(s => s.Email)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(s => s.Address)
                .HasMaxLength(200);

            builder.HasData(new List<Student>()
                {
                new Student
                {
                    Id = 1,
                    Name = "John",
                    Email = "john.doe@seznam.cz",
                    Address = "123 Main St, Prague"
                },
                new Student
                {
                    Id = 2,
                    Name = "Jane",
                    Email = "jane.smith@gmail.com",
                    Address = "456 Elm St, Brno"
                }
            });
        }
    }
}
