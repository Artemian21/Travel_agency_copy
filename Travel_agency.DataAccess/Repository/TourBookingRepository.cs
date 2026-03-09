using Microsoft.EntityFrameworkCore;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Repository
{
    public class TourBookingRepository : ITourBookingRepository
    {
        private TravelAgencyDbContext _context;

        public TourBookingRepository(TravelAgencyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TourBookingEntity>> GetAllTourBookingsAsync()
        {
            return await _context.TourBookings.AsNoTracking().ToListAsync();
        }

        public async Task<TourBookingEntity> GetTourBookingByIdAsync(Guid tourBookingId)
        {
            return await _context.TourBookings.AsNoTracking()
                                               .Include(tb => tb.User)
                                               .Include(tb => tb.Tour)
                                               .FirstOrDefaultAsync(tb => tb.Id == tourBookingId);
        }

        public async Task<TourBookingEntity> AddTourBookingAsync(TourBookingEntity tourBooking)
        {
            _context.TourBookings.Add(tourBooking);
            return tourBooking;
        }

        public async Task<TourBookingEntity> UpdateTourBookingAsync(TourBookingEntity updatedTourBooking)
        {
            var existingTourBooking = await _context.TourBookings.FindAsync(updatedTourBooking.Id);
            if (existingTourBooking == null)
            {
                return null;
            }

            existingTourBooking.UserId = updatedTourBooking.UserId;
            existingTourBooking.TourId = updatedTourBooking.TourId;
            existingTourBooking.Status = updatedTourBooking.Status;
            existingTourBooking.BookingDate = updatedTourBooking.BookingDate;

            return existingTourBooking;
        }

        public async Task DeleteTourBookingAsync(Guid tourBookingId)
        {
            var tourBooking = await _context.TourBookings.FindAsync(tourBookingId);
            if (tourBooking != null)
            {
                _context.TourBookings.Remove(tourBooking);
            }
        }


    }
}
