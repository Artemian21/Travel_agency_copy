using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Configurations
{
    public class TransportEntityConfiguration : IEntityTypeConfiguration<TransportEntity>
    {
        public void Configure(EntityTypeBuilder<TransportEntity> builder)
        {
            builder.ToTable("Transports");
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id).IsRequired();
            builder.Property(t => t.Type).IsRequired().HasMaxLength(50);
            builder.Property(t => t.Company).IsRequired().HasMaxLength(100);
            builder.Property(t => t.DepartureDate).IsRequired();
            builder.Property(t => t.ArrivalDate).IsRequired();
            builder.Property(t => t.Price).IsRequired().HasColumnType("decimal(18,2)");

            builder.HasMany(t => t.TicketBookings)
                .WithOne(tb => tb.Transport)
                .HasForeignKey(tb => tb.TransportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
