using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Repository;

namespace Travel_agency.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TravelAgencyDbContext _context;

        private IHotelRepository _hotelRepository;
        private IHotelBookingRepository _hotelBookingRepository;
        private IHotelRoomRepository _hotelRoomRepository;
        private ITicketBookingRepository _ticketBookingRepository;
        private ITourBookingRepository _tourBookingRepository;
        private ITourRepository _tourRepository;
        private ITransportRepository _transportRepository;
        private IUserRepository _userRepository;
        private bool _disposed = false;

        public UnitOfWork(TravelAgencyDbContext context) => _context = context;

        public IHotelBookingRepository HotelBookings => _hotelBookingRepository ??= new HotelBookingRepository(_context);
        public IHotelRepository Hotels => _hotelRepository ??= new HotelRepository(_context);
        public IHotelRoomRepository HotelRooms => _hotelRoomRepository ??= new HotelRoomRepository(_context);
        public ITicketBookingRepository TicketBookings => _ticketBookingRepository ??= new TicketBookingRepository(_context);
        public ITourBookingRepository TourBookings => _tourBookingRepository ??= new TourBookingRepository(_context);
        public ITourRepository Tours => _tourRepository ??= new TourRepository(_context);
        public ITransportRepository Transports => _transportRepository ??= new TransportRepository(_context);
        public IUserRepository Users => _userRepository ??= new UserRepository(_context);

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}