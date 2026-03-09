using Travel_agency.Core.BusinessModels.Hotels;
using Travel_agency.Core.BusinessModels.Tours;
using Travel_agency.Core.BusinessModels.Transports;

namespace Travel_agency.Core.BusinessModels.Users
{
    public class UserWithBookingsModel : UserModel
    {
        public ICollection<HotelBookingModel> HotelBookings { get; set; } = new List<HotelBookingModel>();
        public ICollection<TicketBookingModel> TicketBookings { get; set; } = new List<TicketBookingModel>();
        public ICollection<TourBookingModel> TourBookings { get; set; } = new List<TourBookingModel>();
    }
}