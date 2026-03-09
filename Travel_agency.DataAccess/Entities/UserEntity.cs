using Travel_agency.Core.Enums;

namespace Travel_agency.DataAccess.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }

        public ICollection<HotelBookingEntity> HotelBookings { get; set; } = new List<HotelBookingEntity>();
        public ICollection<TicketBookingEntity> TicketBookings { get; set; } = new List<TicketBookingEntity>();
        public ICollection<TourBookingEntity> TourBookings { get; set; } = new List<TourBookingEntity>();
    }
}
