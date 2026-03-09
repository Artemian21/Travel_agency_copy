using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Configurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id).IsRequired();
            builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(254);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(u => u.Email).IsUnique();

            builder.HasMany(u => u.HotelBookings)
                .WithOne(hb => hb.User)
                .HasForeignKey(hb => hb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.TicketBookings)
                .WithOne(tb => tb.User)
                .HasForeignKey(tb => tb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.TourBookings)
                .WithOne(tb => tb.User)
                .HasForeignKey(tb => tb.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
