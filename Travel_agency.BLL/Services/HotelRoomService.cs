using AutoMapper;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Hotels;
using Travel_agency.Core.Enums;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.BLL.Services
{
    public class HotelRoomService : IHotelRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HotelRoomService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<HotelRoomModel>> GetAllHotelRoomsAsync()
        {
            var hotelRoomEntities = await _unitOfWork.HotelRooms.GetAllHotelRoomsAsync();
            return _mapper.Map<IEnumerable<HotelRoomModel>>(hotelRoomEntities);
        }

        public async Task<HotelRoomWithBookingModel?> GetHotelRoomByIdAsync(Guid hotelRoomId)
        {
            var hotelRoomEntity = await _unitOfWork.HotelRooms.GetHotelRoomByIdAsync(hotelRoomId);
            if (hotelRoomEntity == null)
            {
                throw new NotFoundException($"Hotel room with ID {hotelRoomId} not found.");
            }

            return _mapper.Map<HotelRoomWithBookingModel>(hotelRoomEntity);
        }

        public async Task<IEnumerable<HotelRoomModel>> GetHotelRoomsByHotelIdAsync(Guid hotelId)
        {
            var hotelEntity = await _unitOfWork.Hotels.GetHotelByIdAsync(hotelId);
            if (hotelEntity == null)
            {
                throw new NotFoundException($"Hotel with ID {hotelId} not found.");
            }

            var hotelRoomEntities = await _unitOfWork.HotelRooms.GetRoomsByHotelIdAsync(hotelId);
            return _mapper.Map<IEnumerable<HotelRoomModel>>(hotelRoomEntities);
        }

        public async Task<HotelRoomModel> AddHotelRoomAsync(HotelRoomModel hotelRoomModel)
        {
            ValidateHotelRoomModel(hotelRoomModel);

            var hotelRoomEntity = _mapper.Map<HotelRoomEntity>(hotelRoomModel);
            var addedHotelRoomEntity = await _unitOfWork.HotelRooms.AddHotelRoomAsync(hotelRoomEntity);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<HotelRoomModel>(addedHotelRoomEntity);
        }

        public async Task<HotelRoomModel> UpdateHotelRoomAsync(HotelRoomModel hotelRoomModel)
        {
            ValidateHotelRoomModel(hotelRoomModel);

            var hotelRoomEntity = _mapper.Map<HotelRoomEntity>(hotelRoomModel);
            var updatedHotelRoomEntity = await _unitOfWork.HotelRooms.UpdateHotelRoomAsync(hotelRoomEntity);
            if (updatedHotelRoomEntity == null)
            {
                throw new NotFoundException($"Hotel room with ID {hotelRoomModel.Id} not found.");
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<HotelRoomModel>(updatedHotelRoomEntity);
        }

        public async Task<bool> DeleteHotelRoomAsync(Guid hotelRoomId)
        {
            var hotelRoomEntity = await _unitOfWork.HotelRooms.GetHotelRoomByIdAsync(hotelRoomId);
            if (hotelRoomEntity == null)
            {
                throw new NotFoundException($"Hotel room with ID {hotelRoomId} not found.");
            }

            await _unitOfWork.HotelRooms.DeleteHotelRoomAsync(hotelRoomId);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private void ValidateHotelRoomModel(HotelRoomModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Hotel room cannot be null.");

            if (!Enum.IsDefined(typeof(RoomType), model.RoomType))
                throw new BusinessValidationException("Invalid room type.");

            if (model.Capacity <= 0)
                throw new BusinessValidationException("Room capacity must be greater than zero.");

            if (model.PricePerNight < 0)
                throw new BusinessValidationException("Price per night cannot be negative.");

            if (model.HotelId == Guid.Empty)
                throw new BusinessValidationException("Hotel ID is required.");
        }
    }
}
