using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Configurations
{
    public class HotelBookingEntityConfiguration : IEntityTypeConfiguration<HotelBookingEntity>
    {
        public void Configure(EntityTypeBuilder<HotelBookingEntity> builder)
        {
            builder.ToTable("HotelBookings");
            builder.HasKey(hb => hb.Id);
            builder.Property(hb => hb.Id).IsRequired();
            builder.Property(hb => hb.StartDate).IsRequired();
            builder.Property(hb => hb.EndDate).IsRequired();
            builder.Property(hb => hb.NumberOfGuests).IsRequired();
            builder.Property(hb => hb.Status).HasConversion<string>().IsRequired().HasMaxLength(50);

            builder.HasOne(hb => hb.User)
                .WithMany(u => u.HotelBookings)
                .HasForeignKey(hb => hb.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(hb => hb.HotelRoom)
                .WithMany(hr => hr.HotelBookings)
                .HasForeignKey(hb => hb.HotelRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
