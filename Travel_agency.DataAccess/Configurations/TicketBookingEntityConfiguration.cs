using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Configurations
{
    public class TicketBookingEntityConfiguration : IEntityTypeConfiguration<TicketBookingEntity>
    {
        public void Configure(EntityTypeBuilder<TicketBookingEntity> builder)
        {
            builder.HasKey(tb => tb.Id);
            builder.Property(tb => tb.Id).IsRequired();
            builder.Property(tb => tb.BookingDate).IsRequired();
            builder.Property(tb => tb.Status)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(tb => tb.Transport)
                .WithMany(t => t.TicketBookings)
                .HasForeignKey(tb => tb.TransportId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(tb => tb.User)
                .WithMany(u => u.TicketBookings)
                .HasForeignKey(tb => tb.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
