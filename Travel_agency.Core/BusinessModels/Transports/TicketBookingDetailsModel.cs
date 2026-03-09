using Travel_agency.Core.BusinessModels.Users;

namespace Travel_agency.Core.BusinessModels.Transports
{
    public class TicketBookingDetailsModel : TicketBookingModel
    {
        public required TransportModel Transport { get; set; }
        public required UserModel User { get; set; }
    }
}
