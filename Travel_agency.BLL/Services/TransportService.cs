using AutoMapper;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Transports;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.BLL.Services
{
    public class TransportService : ITransportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TransportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<IEnumerable<TransportModel>> GetAllTransportsAsync()
        {
            var transportEntities = await _unitOfWork.Transports.GetAllTransportsAsync();
            return _mapper.Map<IEnumerable<TransportModel>>(transportEntities);
        }

        public async Task<TransportWithBookingsModel> GetTransportByIdAsync(Guid transportId)
        {
            var transportEntity = await _unitOfWork.Transports.GetTransportByIdAsync(transportId);
            if (transportEntity == null)
                throw new NotFoundException($"Transport with ID {transportId} not found.");

            return _mapper.Map<TransportWithBookingsModel>(transportEntity);
        }

        public async Task<TransportModel> AddTransportAsync(TransportModel transportModel)
        {
            ValidateTransportModel(transportModel);

            var transportEntity = _mapper.Map<TransportEntity>(transportModel);
            var addedTransportEntity = await _unitOfWork.Transports.AddTransportAsync(transportEntity);

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TransportModel>(addedTransportEntity);
        }

        public async Task<TransportModel> UpdateTransportAsync(TransportModel transportModel)
        {
            ValidateTransportModel(transportModel);

            var transportEntity = _mapper.Map<TransportEntity>(transportModel);
            var updatedTransportEntity = await _unitOfWork.Transports.UpdateTransportAsync(transportEntity);

            if (updatedTransportEntity == null)
                throw new NotFoundException($"Transport with ID {transportModel.Id} not found.");

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TransportModel>(updatedTransportEntity);
        }

        public async Task<bool> DeleteTransportAsync(Guid transportId)
        {
            var transportEntity = await _unitOfWork.Transports.GetTransportByIdAsync(transportId);
            if (transportEntity == null)
            {
                throw new NotFoundException($"Transport with ID {transportId} not found.");
            }

            await _unitOfWork.Transports.DeleteTransportAsync(transportId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private static void ValidateTransportModel(TransportModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.Price < 0)
                throw new BusinessValidationException("Price cannot be negative.");

            if (string.IsNullOrWhiteSpace(model.Company))
                throw new BusinessValidationException("Company name is required.");

            if (string.IsNullOrWhiteSpace(model.Type))
                throw new BusinessValidationException("Transport type is required.");

            if (model.DepartureDate < DateTime.UtcNow)
                throw new BusinessValidationException("Departure date cannot be in the past.");

            if (model.ArrivalDate < model.DepartureDate)
                throw new BusinessValidationException("Arrival date must be after the departure date.");
        }
    }
}
