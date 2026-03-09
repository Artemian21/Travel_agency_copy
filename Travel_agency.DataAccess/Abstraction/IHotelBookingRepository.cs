using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Abstraction
{
    public interface IHotelBookingRepository
    {
        Task<HotelBookingEntity> AddHotelBookingAsync(HotelBookingEntity hotelBooking);
        Task DeleteHotelBookingAsync(Guid hotelBookingId);
        Task<IEnumerable<HotelBookingEntity>> GetAllHotelBookingsAsync();
        Task<HotelBookingEntity> GetHotelBookingByIdAsync(Guid hotelBookingId);
        Task<HotelBookingEntity> UpdateHotelBookingAsync(HotelBookingEntity updatedHotelBooking);
    }
}
