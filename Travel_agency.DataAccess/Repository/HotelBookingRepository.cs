using Microsoft.EntityFrameworkCore;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Repository
{
    public class HotelBookingRepository : IHotelBookingRepository
    {
        private readonly TravelAgencyDbContext _context;

        public HotelBookingRepository(TravelAgencyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HotelBookingEntity>> GetAllHotelBookingsAsync()
        {
            return await _context.HotelBookings.AsNoTracking().ToListAsync();
        }

        public async Task<HotelBookingEntity> GetHotelBookingByIdAsync(Guid hotelBookingId)
        {
            return await _context.HotelBookings
                .AsNoTracking()
                .Include(h => h.User)
                .Include(h => h.HotelRoom)
                .FirstOrDefaultAsync(h => h.Id == hotelBookingId);
        }

        public async Task<HotelBookingEntity> AddHotelBookingAsync(HotelBookingEntity hotelBooking)
        {
            await _context.HotelBookings.AddAsync(hotelBooking);
            return hotelBooking;
        }

        public async Task<HotelBookingEntity> UpdateHotelBookingAsync(HotelBookingEntity updatedHotelBooking)
        {
            var existingHotelBooking = await _context.HotelBookings.FindAsync(updatedHotelBooking.Id);
            if (existingHotelBooking == null)
            {
                return null;
            }

            existingHotelBooking.UserId = updatedHotelBooking.UserId;
            existingHotelBooking.HotelRoomId = updatedHotelBooking.HotelRoomId;
            existingHotelBooking.StartDate = updatedHotelBooking.StartDate;
            existingHotelBooking.EndDate = updatedHotelBooking.EndDate;
            existingHotelBooking.Status = updatedHotelBooking.Status;
            existingHotelBooking.NumberOfGuests = updatedHotelBooking.NumberOfGuests;

            return existingHotelBooking;
        }

        public async Task DeleteHotelBookingAsync(Guid hotelBookingId)
        {
            var hotelBooking = await _context.HotelBookings.FindAsync(hotelBookingId);
            if (hotelBooking != null)
            {
                _context.HotelBookings.Remove(hotelBooking);
            }
        }
    }
}
