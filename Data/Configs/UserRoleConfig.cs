using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace studentsapi.Data.Configs
{
    internal class UserRoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.RoleName)
                .HasMaxLength(250)
                .IsRequired();
            builder.Property(s => s.Description);
            builder.Property(s => s.IsActive)
                .IsRequired();
            builder.Property(s => s.IsDeleted)
                 .IsRequired();
            builder.Property(s => s.CreatedAt)
                .IsRequired();
        }
    }
}
