using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Abstraction
{
    public interface IHotelRoomRepository
    {
        Task<HotelRoomEntity> AddHotelRoomAsync(HotelRoomEntity hotelRoom);
        Task DeleteHotelRoomAsync(Guid hotelRoomId);
        Task<IEnumerable<HotelRoomEntity>> GetAllHotelRoomsAsync();
        Task<HotelRoomEntity> GetHotelRoomByIdAsync(Guid hotelRoomId);
        Task<IEnumerable<HotelRoomEntity>> GetRoomsByHotelIdAsync(Guid hotelId);
        Task<HotelRoomEntity> UpdateHotelRoomAsync(HotelRoomEntity updatedHotelRoom);
    }
}
