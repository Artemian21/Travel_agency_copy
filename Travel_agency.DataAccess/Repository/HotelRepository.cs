using Microsoft.EntityFrameworkCore;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Repository
{
    public class HotelRepository : IHotelRepository
    {
        private readonly TravelAgencyDbContext _context;

        public HotelRepository(TravelAgencyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HotelEntity>> GetAllHotelsAsync()
        {
            return await _context.Hotels.AsNoTracking().ToListAsync();
        }

        public async Task<HotelEntity> GetHotelByIdAsync(Guid hotelId)
        {
            return await _context.Hotels.AsNoTracking()
                                        .Include(h => h.HotelRoom)
                                        .FirstOrDefaultAsync(h => h.Id == hotelId);
        }

        public async Task<HotelEntity> AddHotelAsync(HotelEntity hotel)
        {
            await _context.Hotels.AddAsync(hotel);
            return hotel;
        }

        public async Task<HotelEntity> UpdateHotelAsync(HotelEntity updatedHotel)
        {
            var existingHotel = await _context.Hotels.FindAsync(updatedHotel.Id);
            if (existingHotel == null)
            {
                return null;
            }

            existingHotel.Name = updatedHotel.Name;
            existingHotel.City = updatedHotel.City;
            existingHotel.Address = updatedHotel.Address;
            existingHotel.Country = updatedHotel.Country;

            return existingHotel;
        }

        public async Task DeleteHotelAsync(Guid hotelId)
        {
            var hotel = await _context.Hotels.FindAsync(hotelId);
            if (hotel != null)
            {
                _context.Hotels.Remove(hotel);
            }
        }
    }
}
