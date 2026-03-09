using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Configurations
{
    public class TourBookingEntityConfiguration : IEntityTypeConfiguration<TourBookingEntity>
    {
        public void Configure(EntityTypeBuilder<TourBookingEntity> builder)
        {
            builder.HasKey(tb => tb.Id);
            builder.Property(tb => tb.Id).IsRequired();
            builder.Property(tb => tb.BookingDate).IsRequired();
            builder.Property(tb => tb.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(tb => tb.Tour)
                .WithMany(t => t.TourBookings)
                .HasForeignKey(tb => tb.TourId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tb => tb.User)
                .WithMany(u => u.TourBookings)
                .HasForeignKey(tb => tb.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
