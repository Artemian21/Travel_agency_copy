using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Configurations
{
    public class TourEntityConfiguration : IEntityTypeConfiguration<TourEntity>
    {
        public void Configure(EntityTypeBuilder<TourEntity> builder)
        {
            builder.ToTable("Tours");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).IsRequired();
            builder.Property(t => t.Name).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Type)
                .HasConversion<string>()
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(t => t.Country).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Region).IsRequired().HasMaxLength(100);
            builder.Property(t => t.Price).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(t => t.StartDate).IsRequired();
            builder.Property(t => t.EndDate).IsRequired();
            builder.Property(t => t.ImageUrl).HasMaxLength(255);

            builder.HasMany(t => t.TourBookings)
                .WithOne(tb => tb.Tour)
                .HasForeignKey(tb => tb.TourId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
