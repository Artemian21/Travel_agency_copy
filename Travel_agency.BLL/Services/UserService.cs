using AutoMapper;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Users;
using Travel_agency.Core.Enums;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
        }

        public async Task<bool> AssignUserRoleAsync(Guid userId, UserRole? role)
        {
            var userEntity = await _unitOfWork.Users.GetUserByIdAsync(userId);
            if (userEntity == null)
            {
                throw new NotFoundException($"User with ID {userId} not found.");
            }

            userEntity.Role = (UserRole)role;
            await _unitOfWork.Users.UpdateUserAsync(userEntity);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var userEntity = await _unitOfWork.Users.GetUserByIdAsync(userId);
            if (userEntity == null)
            {
                throw new NotFoundException($"User with ID {userId} not found.");
            }

            await _unitOfWork.Users.DeleteUserAsync(userId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<UserModel>> GetAllUserAsync()
        {
            var userEntities = await _unitOfWork.Users.GetAllUsers();
            return _mapper.Map<IEnumerable<UserModel>>(userEntities);
        }

        public async Task<UserWithBookingsModel> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new BusinessValidationException("Email cannot be empty.");
            }

            var userEntity = await _unitOfWork.Users.GetUserByEmailAsync(email);
            if (userEntity == null)
            {
                throw new NotFoundException($"User with email '{email}' not found.");
            }

            return _mapper.Map<UserWithBookingsModel>(userEntity);
        }

        public async Task<UserWithBookingsModel> GetUserByIdAsync(Guid userId)
        {
            var userEntity = await _unitOfWork.Users.GetUserByIdAsync(userId);
            if (userEntity == null)
            {
                throw new NotFoundException($"User with ID {userId} not found.");
            }

            return _mapper.Map<UserWithBookingsModel>(userEntity);
        }

        public async Task<UserModel> UpdateUserProfileAsync(UserModel updateModel)
        {
            ValidateUserModel(updateModel);

            var userEntity = await _unitOfWork.Users.GetUserByIdAsync(updateModel.Id);
            var updatedUserEntity = await _unitOfWork.Users.UpdateUserAsync(_mapper.Map<UserEntity>(updateModel));
            if (updatedUserEntity == null)
            {
                throw new NotFoundException($"User with ID {updateModel.Id} not found.");
            }

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserModel>(updatedUserEntity);
        }

        private void ValidateUserModel(UserModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "User object cannot be null.");

            if (string.IsNullOrWhiteSpace(model.Username))
                throw new BusinessValidationException("Username is required.");

            if (string.IsNullOrWhiteSpace(model.Email))
                throw new BusinessValidationException("Email is required.");

            if (!Enum.IsDefined(typeof(UserRole), model.Role))
                throw new BusinessValidationException("Invalid user role.");
        }
    }
}
