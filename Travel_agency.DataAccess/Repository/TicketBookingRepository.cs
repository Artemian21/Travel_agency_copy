using Microsoft.EntityFrameworkCore;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Repository
{
    public class TicketBookingRepository : ITicketBookingRepository
    {
        private readonly TravelAgencyDbContext _context;

        public TicketBookingRepository(TravelAgencyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TicketBookingEntity>> GetAllTicketBookingsAsync()
        {
            return await _context.TicketBookings.AsNoTracking().ToListAsync();
        }

        public async Task<TicketBookingEntity> GetTicketBookingByIdAsync(Guid ticketBookingId)
        {
            return await _context.TicketBookings
                .AsNoTracking()
                .Include(tb => tb.User)
                .Include(tb => tb.Transport)
                .FirstOrDefaultAsync(tb => tb.Id == ticketBookingId);
        }

        public async Task<TicketBookingEntity> AddTicketBookingAsync(TicketBookingEntity ticketBooking)
        {
            await _context.TicketBookings.AddAsync(ticketBooking);
            return ticketBooking;
        }

        public async Task<TicketBookingEntity> UpdateTicketBookingAsync(TicketBookingEntity updatedTicketBooking)
        {
            var existingTicketBooking = await _context.TicketBookings.FindAsync(updatedTicketBooking.Id);
            if (existingTicketBooking == null)
            {
                return null;
            }

            existingTicketBooking.UserId = updatedTicketBooking.UserId;
            existingTicketBooking.TransportId = updatedTicketBooking.TransportId;
            existingTicketBooking.Status = updatedTicketBooking.Status;
            existingTicketBooking.BookingDate = updatedTicketBooking.BookingDate;

            return existingTicketBooking;
        }

        public async Task DeleteTicketBookingAsync(Guid ticketBookingId)
        {
            var ticketBooking = await _context.TicketBookings.FindAsync(ticketBookingId);
            if (ticketBooking != null)
            {
                _context.TicketBookings.Remove(ticketBooking);
            }
        }
    }
}
