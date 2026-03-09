using Microsoft.EntityFrameworkCore;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess
{
    public class TravelAgencyDbContext : DbContext
    {
        public TravelAgencyDbContext(DbContextOptions<TravelAgencyDbContext> options)
        : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TravelAgencyDbContext).Assembly);
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<TourEntity> Tours { get; set; }
        public DbSet<TourBookingEntity> TourBookings { get; set; }
        public DbSet<HotelEntity> Hotels { get; set; }
        public DbSet<HotelRoomEntity> HotelRooms { get; set; }
        public DbSet<HotelBookingEntity> HotelBookings { get; set; }
        public DbSet<TransportEntity> Transports { get; set; }
        public DbSet<TicketBookingEntity> TicketBookings { get; set; }
    }
}
