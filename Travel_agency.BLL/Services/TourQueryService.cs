using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Tours;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.BLL.Services;

public class TourQueryService : ITourQueryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TourQueryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TourModel>> GetFilteredToursAsync(TourFilterModel filter)
    {
        var query = _unitOfWork.Tours.Query(); // IQueryable<TourEntity>

        query = ApplyFilters(query, filter);

        return await query
            .ProjectTo<TourModel>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    private static IQueryable<TourEntity> ApplyFilters(
        IQueryable<TourEntity> query,
        TourFilterModel filter)
    {
        if (!string.IsNullOrWhiteSpace(filter.Country))
        {
            var country = filter.Country.Trim();
            query = query.Where(t => t.Country == country);
        }

        if (filter.Type.HasValue)
            query = query.Where(t => t.Type == filter.Type.Value);

        if (!string.IsNullOrWhiteSpace(filter.Region))
        {
            var region = filter.Region.Trim();
            query = query.Where(t => t.Region == region);
        }

        if (!string.IsNullOrWhiteSpace(filter.Name))
        {
            var name = filter.Name.Trim();
            query = query.Where(t => t.Name.Contains(name));
        }

        if (filter.StartDateFrom.HasValue)
            query = query.Where(t => t.StartDate >= filter.StartDateFrom.Value);

        if (filter.StartDateTo.HasValue)
            query = query.Where(t => t.StartDate <= filter.StartDateTo.Value);

        if (filter.Price.HasValue)
            query = query.Where(t => t.Price <= filter.Price.Value);

        return query;
    }

    public async Task<IEnumerable<TourModel>> SearchToursAsync(string searchQuery)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
            return Enumerable.Empty<TourModel>();

        var search = searchQuery.Trim();

        var query = _unitOfWork.Tours.Query();

        query = query.Where(t =>
            t.Name.Contains(search) ||
            t.Type.ToString().Contains(search) ||
            t.Country.Contains(search) ||
            t.Region.Contains(search)
        );

        if (DateTime.TryParse(search, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            query = query.Where(t => t.StartDate.Date == parsedDate.Date);

        return await query
            .ProjectTo<TourModel>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}