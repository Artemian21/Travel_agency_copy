using Microsoft.EntityFrameworkCore;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Repository
{
    public class HotelRoomRepository : IHotelRoomRepository
    {
        private readonly TravelAgencyDbContext _context;

        public HotelRoomRepository(TravelAgencyDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HotelRoomEntity>> GetAllHotelRoomsAsync()
        {
            return await _context.HotelRooms.AsNoTracking().ToListAsync();
        }

        public async Task<HotelRoomEntity> GetHotelRoomByIdAsync(Guid hotelRoomId)
        {
            return await _context.HotelRooms.AsNoTracking()
                                            .Include(h => h.Hotel)
                                            .Include(h => h.HotelBookings)
                                            .FirstOrDefaultAsync(h => h.Id == hotelRoomId);
        }

        public async Task<IEnumerable<HotelRoomEntity>> GetRoomsByHotelIdAsync(Guid hotelId)
        {
            return await _context.HotelRooms.AsNoTracking()
                .Where(r => r.HotelId == hotelId)
                .ToListAsync();
        }

        public async Task<HotelRoomEntity> AddHotelRoomAsync(HotelRoomEntity hotelRoom)
        {
            await _context.HotelRooms.AddAsync(hotelRoom);
            return hotelRoom;
        }

        public async Task<HotelRoomEntity> UpdateHotelRoomAsync(HotelRoomEntity updatedHotelRoom)
        {
            var existingHotelRoom = await _context.HotelRooms.FindAsync(updatedHotelRoom.Id);
            if (existingHotelRoom == null)
            {
                return null;
            }

            existingHotelRoom.RoomType = updatedHotelRoom.RoomType;
            existingHotelRoom.PricePerNight = updatedHotelRoom.PricePerNight;
            existingHotelRoom.Capacity = updatedHotelRoom.Capacity;
            existingHotelRoom.HotelId = updatedHotelRoom.HotelId;

            return existingHotelRoom;
        }

        public async Task DeleteHotelRoomAsync(Guid hotelRoomId)
        {
            var hotelRoom = await _context.HotelRooms.FindAsync(hotelRoomId);
            if (hotelRoom != null)
            {
                _context.HotelRooms.Remove(hotelRoom);
            }
        }
    }
}
