using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Abstraction
{
    public interface ITicketBookingRepository
    {
        Task<TicketBookingEntity> AddTicketBookingAsync(TicketBookingEntity ticketBooking);
        Task DeleteTicketBookingAsync(Guid ticketBookingId);
        Task<IEnumerable<TicketBookingEntity>> GetAllTicketBookingsAsync();
        Task<TicketBookingEntity> GetTicketBookingByIdAsync(Guid ticketBookingId);
        Task<TicketBookingEntity> UpdateTicketBookingAsync(TicketBookingEntity updatedTicketBooking);
    }

}
