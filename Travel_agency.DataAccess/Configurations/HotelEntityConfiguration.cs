using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Configurations
{
    public class HotelEntityConfiguration : IEntityTypeConfiguration<HotelEntity>
    {
        public void Configure(EntityTypeBuilder<HotelEntity> builder)
        {
            builder.ToTable("Hotels");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Name).IsRequired().HasMaxLength(100);
            builder.Property(h => h.Country).IsRequired().HasMaxLength(50);
            builder.Property(h => h.City).IsRequired().HasMaxLength(50);
            builder.Property(h => h.Address).IsRequired().HasMaxLength(200);

            builder.HasMany(h => h.HotelRoom)
                .WithOne(hr => hr.Hotel)
                .HasForeignKey(hr => hr.HotelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
