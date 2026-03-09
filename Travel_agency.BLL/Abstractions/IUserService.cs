using Travel_agency.Core.BusinessModels.Users;
using Travel_agency.Core.Enums;

namespace Travel_agency.BLL.Abstractions
{
    public interface IUserService
    {
        Task<bool> AssignUserRoleAsync(Guid userId, UserRole? role);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<IEnumerable<UserModel>> GetAllUserAsync();
        Task<UserWithBookingsModel> GetUserByEmailAsync(string email);
        Task<UserWithBookingsModel> GetUserByIdAsync(Guid userId);
        Task<UserModel> UpdateUserProfileAsync(UserModel updateModel);
    }
}
