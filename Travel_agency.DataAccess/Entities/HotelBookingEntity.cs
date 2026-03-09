using Travel_agency.Core.Enums;

namespace Travel_agency.DataAccess.Entities
{
    public class HotelBookingEntity
    {
        public Guid Id { get; set; }
        public Guid HotelRoomId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfGuests { get; set; }
        public Status Status { get; set; }
        public required HotelRoomEntity HotelRoom { get; set; }
        public required UserEntity User { get; set; }
    }
}