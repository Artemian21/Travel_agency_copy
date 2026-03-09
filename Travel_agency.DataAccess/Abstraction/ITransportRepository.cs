using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Abstraction
{
    public interface ITransportRepository
    {
        Task<TransportEntity> AddTransportAsync(TransportEntity transport);
        Task DeleteTransportAsync(Guid transportId);
        Task<IEnumerable<TransportEntity>> GetAllTransportsAsync();
        Task<TransportEntity> GetTransportByIdAsync(Guid transportId);
        Task<TransportEntity> UpdateTransportAsync(TransportEntity updatedTransport);
    }
}
