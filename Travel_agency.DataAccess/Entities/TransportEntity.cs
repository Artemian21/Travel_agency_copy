namespace Travel_agency.DataAccess.Entities
{
    public class TransportEntity
    {
        public Guid Id { get; set; }
        public required string Type { get; set; }
        public required string Company { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }
        public decimal Price { get; set; }
        public ICollection<TicketBookingEntity> TicketBookings { get; set; } = new List<TicketBookingEntity>();
    }
}
