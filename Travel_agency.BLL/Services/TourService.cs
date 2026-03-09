using AutoMapper;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Tours;
using Travel_agency.Core.Enums;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.BLL.Services
{
    public class TourService : ITourService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TourService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<TourModel>> GetAllToursAsync()
        {
            var tourEntities = await _unitOfWork.Tours.GetAllToursAsync();
            return _mapper.Map<IEnumerable<TourModel>>(tourEntities);
        }

        public async Task<PagedResult<TourModel>> GetPagedToursAsync(int pageNumber, int pageSize)
        {
            var totalCount = await _unitOfWork.Tours.GetTotalToursCountAsync();
            var tours = await _unitOfWork.Tours.GetToursPagedAsync(pageNumber, pageSize);
            var mappedTours = _mapper.Map<IEnumerable<TourModel>>(tours);

            return new PagedResult<TourModel>
            {
                Items = mappedTours,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };
        }

        public async Task<TourWithBookingsModel> GetTourByIdAsync(Guid tourId)
        {
            var tourEntity = await _unitOfWork.Tours.GetTourByIdAsync(tourId);
            if (tourEntity == null)
                throw new NotFoundException($"Tour with ID {tourId} not found.");

            return _mapper.Map<TourWithBookingsModel>(tourEntity);
        }

        public async Task<TourModel> AddTourAsync(TourModel tourModel)
        {
            ValidateTourModel(tourModel);

            var tourEntity = _mapper.Map<TourEntity>(tourModel);
            var addedTourEntity = await _unitOfWork.Tours.AddTourAsync(tourEntity);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TourModel>(addedTourEntity);
        }

        public async Task<TourModel> UpdateTourAsync(TourModel tourModel)
        {
            ValidateTourModel(tourModel);

            var tourEntity = _mapper.Map<TourEntity>(tourModel);
            var updatedTourEntity = await _unitOfWork.Tours.UpdateTourAsync(tourEntity);

            if (updatedTourEntity == null)
                throw new NotFoundException($"Tour with ID {tourModel.Id} not found.");

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TourModel>(updatedTourEntity);
        }

        public async Task<bool> DeleteTourAsync(Guid tourId)
        {
            var tourEntity = await _unitOfWork.Tours.GetTourByIdAsync(tourId);
            if (tourEntity == null)
            {
                throw new NotFoundException($"Tour with ID {tourId} not found.");
            }

            await _unitOfWork.Tours.DeleteTourAsync(tourId);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static void ValidateTourModel(TourModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new BusinessValidationException("Tour name is required.");

            if (!Enum.IsDefined(typeof(TypeTour), model.Type))
                throw new BusinessValidationException("Invalid tour type.");

            if (string.IsNullOrWhiteSpace(model.Country))
                throw new BusinessValidationException("Country is required.");

            if (string.IsNullOrWhiteSpace(model.Region))
                throw new BusinessValidationException("Region is required.");

            if (model.StartDate.Date < DateTime.UtcNow.Date)
                throw new BusinessValidationException("Start date cannot be in the past.");

            if (model.EndDate.Date < model.StartDate.Date)
                throw new BusinessValidationException("End date must be after the start date.");

            if (model.Price < 0)
                throw new BusinessValidationException("Price cannot be negative.");
        }
    }
}
