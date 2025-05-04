using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace studentsapi.Data.Configs
{
    internal class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Property(s => s.UserName)
                .IsRequired()
                .HasMaxLength(250);
            builder.Property(s => s.Password)
                .IsRequired();
            builder.Property(s => s.PasswordSalt)
                .IsRequired();
            builder.Property(s => s.UserType)
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
