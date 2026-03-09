using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Abstraction
{
    public interface ITourBookingRepository
    {
        Task<TourBookingEntity> AddTourBookingAsync(TourBookingEntity tourBooking);
        Task DeleteTourBookingAsync(Guid tourBookingId);
        Task<IEnumerable<TourBookingEntity>> GetAllTourBookingsAsync();
        Task<TourBookingEntity> GetTourBookingByIdAsync(Guid tourBookingId);
        Task<TourBookingEntity> UpdateTourBookingAsync(TourBookingEntity updatedTourBooking);
    }
}
