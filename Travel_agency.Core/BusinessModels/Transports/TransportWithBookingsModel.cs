namespace Travel_agency.Core.BusinessModels.Transports
{
    public class TransportWithBookingsModel : TransportModel
    {
        public ICollection<TicketBookingModel> TicketBookings { get; set; } = new List<TicketBookingModel>();
    }
}
