using Microsoft.EntityFrameworkCore;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.DataAccess.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly TravelAgencyDbContext _context;

        public UserRepository(TravelAgencyDbContext context)
        {
            _context = context;
        }

        public async Task<UserEntity> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.AsNoTracking()
                                       .Include(u => u.HotelBookings)
                                       .Include(u => u.TicketBookings)
                                       .Include(u => u.TourBookings)
                                       .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<IEnumerable<UserEntity>> GetAllUsers()
        {
            return await _context.Users.AsNoTracking()
                .ToListAsync();
        }

        public async Task<UserEntity?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<UserEntity> AddUserAsync(UserEntity user)
        {
            _context.Users.Add(user);
            return user;
        }

        public async Task<UserEntity> UpdateUserAsync(UserEntity updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(updatedUser.Id);
            if (existingUser == null)
            {
                return null;
            }

            existingUser.Username = updatedUser.Username;
            existingUser.Email = updatedUser.Email;
            existingUser.Role = updatedUser.Role;

            return existingUser;
        }

        public async Task DeleteUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }


    }
}
