using AutoMapper;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Transports;
using Travel_agency.Core.Enums;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.BLL.Services
{
    public class TicketBookingService : ITicketBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TicketBookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<TicketBookingModel>> GetAllTicketBookingsAsync()
        {
            var ticketBookingEntities = await _unitOfWork.TicketBookings.GetAllTicketBookingsAsync();
            return _mapper.Map<IEnumerable<TicketBookingModel>>(ticketBookingEntities);
        }

        public async Task<TicketBookingDetailsModel?> GetTicketBookingByIdAsync(Guid ticketBookingId)
        {
            var ticketBookingEntity = await _unitOfWork.TicketBookings.GetTicketBookingByIdAsync(ticketBookingId);
            if (ticketBookingEntity == null)
                throw new NotFoundException($"Ticket Booking with ID {ticketBookingId} not found.");

            return _mapper.Map<TicketBookingDetailsModel>(ticketBookingEntity);
        }

        public async Task<TicketBookingModel> AddTicketBookingAsync(TicketBookingModel ticketBookingModel)
        {
            ValidateTicketBookingModel(ticketBookingModel);

            var ticketBookingEntity = _mapper.Map<TicketBookingEntity>(ticketBookingModel);
            var addedTicketBookingEntity = await _unitOfWork.TicketBookings.AddTicketBookingAsync(ticketBookingEntity);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TicketBookingModel>(addedTicketBookingEntity);
        }

        public async Task<TicketBookingModel> UpdateTicketBookingAsync(TicketBookingModel ticketBookingModel)
        {
            ValidateTicketBookingModel(ticketBookingModel);

            var ticketBookingEntity = _mapper.Map<TicketBookingEntity>(ticketBookingModel);
            var updatedTicketBookingEntity = await _unitOfWork.TicketBookings.UpdateTicketBookingAsync(ticketBookingEntity);
            if (updatedTicketBookingEntity == null)
                throw new NotFoundException($"Ticket Booking with ID {ticketBookingModel.Id} not found.");

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TicketBookingModel>(updatedTicketBookingEntity);
        }

        public async Task<bool> DeleteTicketBookingAsync(Guid ticketBookingId)
        {
            var ticketBookingEntity = await _unitOfWork.TicketBookings.GetTicketBookingByIdAsync(ticketBookingId);
            if (ticketBookingEntity == null)
            {
                throw new NotFoundException($"Ticket Booking with ID {ticketBookingId} not found.");
            }

            await _unitOfWork.TicketBookings.DeleteTicketBookingAsync(ticketBookingId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private void ValidateTicketBookingModel(TicketBookingModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Ticket booking cannot be null.");

            if (model.TransportId == Guid.Empty)
                throw new BusinessValidationException("TransportId is required.");

            if (model.UserId == Guid.Empty)
                throw new BusinessValidationException("UserId is required.");

            if (!Enum.IsDefined(typeof(Status), model.Status))
                throw new BusinessValidationException("Invalid booking status.");
        }
    }
}
