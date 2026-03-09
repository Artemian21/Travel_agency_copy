using AutoMapper;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Tours;
using Travel_agency.Core.Enums;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.BLL.Services
{
    public class TourBookingService : ITourBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TourBookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<TourBookingModel>> GetAllTourBookingsAsync()
        {
            var tourBookingEntities = await _unitOfWork.TourBookings.GetAllTourBookingsAsync();
            return _mapper.Map<IEnumerable<TourBookingModel>>(tourBookingEntities);
        }

        public async Task<TourBookingDetailsModel?> GetTourBookingByIdAsync(Guid tourBookingId)
        {
            var tourBookingEntity = await _unitOfWork.TourBookings.GetTourBookingByIdAsync(tourBookingId);
            if (tourBookingEntity == null)
                throw new NotFoundException($"Tour booking with ID {tourBookingId} not found.");


            return _mapper.Map<TourBookingDetailsModel>(tourBookingEntity);
        }

        public async Task<TourBookingModel> AddTourBookingAsync(TourBookingModel tourBookingModel)
        {
            ValidateTourBookingModel(tourBookingModel);

            var tourBookingEntity = _mapper.Map<TourBookingEntity>(tourBookingModel);
            var addedTourBookingEntity = await _unitOfWork.TourBookings.AddTourBookingAsync(tourBookingEntity);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TourBookingModel>(addedTourBookingEntity);
        }

        public async Task<TourBookingModel> UpdateTourBookingAsync(TourBookingModel tourBookingModel)
        {
            ValidateTourBookingModel(tourBookingModel);

            var tourBookingEntity = _mapper.Map<TourBookingEntity>(tourBookingModel);
            var updatedTourBookingEntity = await _unitOfWork.TourBookings.UpdateTourBookingAsync(tourBookingEntity);
            if (updatedTourBookingEntity == null)
                throw new NotFoundException($"Tour booking with ID {tourBookingModel.Id} not found.");

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TourBookingModel>(updatedTourBookingEntity);
        }

        public async Task<bool> DeleteTourBookingAsync(Guid tourBookingId)
        {
            var tourBookingEntity = await _unitOfWork.TourBookings.GetTourBookingByIdAsync(tourBookingId);
            if (tourBookingEntity == null)
            {
                throw new NotFoundException($"Tour booking with ID {tourBookingId} not found.");
            }

            await _unitOfWork.TourBookings.DeleteTourBookingAsync(tourBookingId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private void ValidateTourBookingModel(TourBookingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Tour booking cannot be null.");

            if (model.TourId == Guid.Empty)
                throw new BusinessValidationException("TourId is required.");

            if (model.UserId == Guid.Empty)
                throw new BusinessValidationException("UserId is required.");

            if (!Enum.IsDefined(typeof(Status), model.Status))
                throw new BusinessValidationException("Invalid booking status.");
        }
    }
}
