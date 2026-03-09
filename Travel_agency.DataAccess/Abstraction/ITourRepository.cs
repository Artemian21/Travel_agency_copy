using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Abstraction
{
    public interface ITourRepository
    {
        Task<TourEntity> AddTourAsync(TourEntity tour);
        Task DeleteTourAsync(Guid tourId);
        Task<IEnumerable<TourEntity>> GetAllToursAsync();
        Task<IEnumerable<TourEntity>> GetToursPagedAsync(int pageNumber, int pageSize);
        Task<int> GetTotalToursCountAsync();
        Task<TourEntity> GetTourByIdAsync(Guid tourId);
        Task<TourEntity> UpdateTourAsync(TourEntity updatedTour);
        IQueryable<TourEntity> Query();
    }
}
