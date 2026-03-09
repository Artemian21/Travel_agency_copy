using AutoMapper;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Hotels;
using Travel_agency.Core.Enums;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.BLL.Services
{
    public class HotelBookingService : IHotelBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HotelBookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<HotelBookingModel>> GetAllHotelBookingsAsync()
        {
            var hotelBookingsEntities = await _unitOfWork.HotelBookings.GetAllHotelBookingsAsync();
            return _mapper.Map<IEnumerable<HotelBookingModel>>(hotelBookingsEntities);
        }

        public async Task<HotelBookingDetailsModel> GetHotelBookingByIdAsync(Guid hotelBookingId)
        {
            var hotelBookingEntity = await _unitOfWork.HotelBookings.GetHotelBookingByIdAsync(hotelBookingId);
            if (hotelBookingEntity == null)
            {
                throw new NotFoundException($"Hotel booking with ID {hotelBookingId} not found.");
            }

            return _mapper.Map<HotelBookingDetailsModel>(hotelBookingEntity);
        }

        public async Task<HotelBookingModel> AddHotelBookingAsync(HotelBookingModel hotelBookingModel)
        {
            ValidateHotelBookingModel(hotelBookingModel);

            var hotelBookingEntity = _mapper.Map<HotelBookingEntity>(hotelBookingModel);
            var addedHotelBookingEntity = await _unitOfWork.HotelBookings.AddHotelBookingAsync(hotelBookingEntity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<HotelBookingModel>(addedHotelBookingEntity);
        }

        public async Task<HotelBookingModel> UpdateHotelBookingAsync(HotelBookingModel hotelBookingModel)
        {
            ValidateHotelBookingModel(hotelBookingModel);

            var hotelBookingEntity = _mapper.Map<HotelBookingEntity>(hotelBookingModel);
            var updatedHotelBookingEntity = await _unitOfWork.HotelBookings.UpdateHotelBookingAsync(hotelBookingEntity);
            if (updatedHotelBookingEntity == null)
            {
                throw new NotFoundException($"Hotel booking with ID {hotelBookingModel.Id} not found.");
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<HotelBookingModel>(updatedHotelBookingEntity);
        }

        public async Task<bool> DeleteHotelBookingAsync(Guid hotelBookingId)
        {
            var hotelBookingEntity = await _unitOfWork.HotelBookings.GetHotelBookingByIdAsync(hotelBookingId);
            if (hotelBookingEntity == null)
            {
                throw new NotFoundException($"Hotel booking with ID {hotelBookingId} not found.");
            }

            await _unitOfWork.HotelBookings.DeleteHotelBookingAsync(hotelBookingId);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private void ValidateHotelBookingModel(HotelBookingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Booking object cannot be null.");

            if (model.HotelRoomId == Guid.Empty)
                throw new BusinessValidationException("Hotel room ID is required.");

            if (model.UserId == Guid.Empty)
                throw new BusinessValidationException("User ID is required.");

            if (model.StartDate.Date < DateTime.UtcNow.Date)
                throw new BusinessValidationException("Start date cannot be in the past.");

            if (model.EndDate.Date <= model.StartDate.Date)
                throw new BusinessValidationException("End date must be after start date.");

            if (model.NumberOfGuests <= 0)
                throw new BusinessValidationException("Number of guests must be greater than 0.");

            if (!Enum.IsDefined(typeof(Status), model.Status))
                throw new BusinessValidationException("Invalid booking status.");
        }
    }
}
