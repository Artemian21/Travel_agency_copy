using AutoMapper;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Hotels;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.BLL.Services
{
    public class HotelService : IHotelService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HotelService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<HotelModel>> GetAllHotelsAsync()
        {
            var hotelEntities = await _unitOfWork.Hotels.GetAllHotelsAsync();
            return _mapper.Map<IEnumerable<HotelModel>>(hotelEntities);
        }

        public async Task<HotelWithBookingsModel?> GetHotelByIdAsync(Guid hotelId)
        {
            var hotelEntity = await _unitOfWork.Hotels.GetHotelByIdAsync(hotelId);
            if (hotelEntity == null)
                throw new NotFoundException($"Hotel with ID {hotelId} not found.");

            return _mapper.Map<HotelWithBookingsModel>(hotelEntity);
        }

        public async Task<HotelModel> AddHotelAsync(HotelModel hotelModel)
        {
            ValidateHotelModel(hotelModel);

            var hotelEntity = _mapper.Map<HotelEntity>(hotelModel);
            var addedHotelEntity = await _unitOfWork.Hotels.AddHotelAsync(hotelEntity);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<HotelModel>(addedHotelEntity);
        }

        public async Task<HotelModel> UpdateHotelAsync(HotelModel hotelModel)
        {
            ValidateHotelModel(hotelModel);

            var hotelEntity = _mapper.Map<HotelEntity>(hotelModel);
            var updatedHotelEntity = await _unitOfWork.Hotels.UpdateHotelAsync(hotelEntity);
            if (updatedHotelEntity == null)
                throw new NotFoundException($"Hotel with ID {hotelModel.Id} not found.");

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<HotelModel>(updatedHotelEntity);
        }

        public async Task<bool> DeleteHotelAsync(Guid hotelId)
        {
            var hotelEntity = await _unitOfWork.Hotels.GetHotelByIdAsync(hotelId);
            if (hotelEntity == null)
            {
                throw new NotFoundException($"Hotel with ID {hotelId} not found.");
            }

            await _unitOfWork.Hotels.DeleteHotelAsync(hotelId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private void ValidateHotelModel(HotelModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Hotel object cannot be null.");

            if (string.IsNullOrWhiteSpace(model.Name))
                throw new BusinessValidationException("Hotel name is required.");

            if (string.IsNullOrWhiteSpace(model.Country))
                throw new BusinessValidationException("Country is required.");

            if (string.IsNullOrWhiteSpace(model.City))
                throw new BusinessValidationException("City is required.");

            if (string.IsNullOrWhiteSpace(model.Address))
                throw new BusinessValidationException("Address is required.");
        }
    }
}
