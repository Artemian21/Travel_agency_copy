using Microsoft.EntityFrameworkCore;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Repository;

public class TourRepository : ITourRepository
{
    private readonly TravelAgencyDbContext _context;
    private DbSet<TourEntity> Tours => _context.Tours;

    public TourRepository(TravelAgencyDbContext context)
    {
        _context = context;
    }

    public IQueryable<TourEntity> Query()
        => Tours.AsNoTracking();

    public async Task<IEnumerable<TourEntity>> GetAllToursAsync()
        => await Query().ToListAsync();

    public async Task<IEnumerable<TourEntity>> GetToursPagedAsync(int pageNumber, int pageSize)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        return await Query()
            .OrderBy(t => t.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<int> GetTotalToursCountAsync()
        => Tours.CountAsync();

    public Task<TourEntity?> GetTourByIdAsync(Guid tourId)
        => Tours
            .AsNoTracking()
            .Include(t => t.TourBookings)
            .FirstOrDefaultAsync(t => t.Id == tourId);

    public Task<TourEntity> AddTourAsync(TourEntity tour)
    {
        Tours.Add(tour);
        return Task.FromResult(tour);
    }

    public Task<TourEntity?> UpdateTourAsync(TourEntity updatedTour)
    {
        Tours.Update(updatedTour);
        return Task.FromResult<TourEntity?>(updatedTour);
    }

    public async Task DeleteTourAsync(Guid tourId)
    {
        var entity = await Tours.FindAsync(tourId);
        if (entity != null)
            Tours.Remove(entity);
    }
}