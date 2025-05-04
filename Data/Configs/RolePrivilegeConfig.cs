using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace studentsapi.Data.Configs
{
    internal class RolePrivilegeConfig : IEntityTypeConfiguration<RolePrivilege>
    {
        public void Configure(EntityTypeBuilder<RolePrivilege> builder)
        {
            builder.ToTable("RolesPrivileges");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.RoleId)
                .IsRequired();
            builder.Property(s => s.RolePrivilegeName)
                .HasMaxLength(250)
                .IsRequired();
            builder.Property(s => s.IsActive)
                .IsRequired();
            builder.Property(s => s.IsDeleted)
                 .IsRequired();
            builder.Property(s => s.CreatedAt)
                .IsRequired();
        }
    }
}