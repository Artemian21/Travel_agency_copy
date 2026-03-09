using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Configurations
{
    public class HotelRoomEntityConfiguration : IEntityTypeConfiguration<HotelRoomEntity>
    {
        public void Configure(EntityTypeBuilder<HotelRoomEntity> builder)
        {
            builder.ToTable("HotelRooms");
            builder.HasKey(hr => hr.Id);
            builder.Property(hr => hr.Id).IsRequired();
            builder.Property(hr => hr.RoomType)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(hr => hr.PricePerNight).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(hr => hr.Capacity).IsRequired();

            builder.HasOne(hr => hr.Hotel)
                .WithMany(h => h.HotelRoom)
                .HasForeignKey(hr => hr.HotelId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(hr => hr.HotelBookings)
                .WithOne(hb => hb.HotelRoom)
                .HasForeignKey(hb => hb.HotelRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
