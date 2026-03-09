using Travel_agency.Core.Enums;

namespace Travel_agency.DataAccess.Entities
{
    public class TourBookingEntity
    {
        public Guid Id { get; set; }
        public Guid TourId { get; set; }
        public Guid UserId { get; set; }
        public DateTime BookingDate { get; set; }
        public Status Status { get; set; }
        public required TourEntity Tour { get; set; }
        public required UserEntity User { get; set; }
    }
}