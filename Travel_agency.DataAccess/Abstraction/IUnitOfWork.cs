using System;

namespace Travel_agency.DataAccess.Abstraction
{
    public interface IUnitOfWork : IDisposable
    {
        IHotelBookingRepository HotelBookings { get; }
        IHotelRoomRepository HotelRooms { get; }
        IHotelRepository Hotels { get; }
        ITicketBookingRepository TicketBookings { get; }
        ITourBookingRepository TourBookings { get; }
        ITourRepository Tours { get; }
        ITransportRepository Transports { get; }
        IUserRepository Users { get; }

        Task SaveChangesAsync();
    }
}