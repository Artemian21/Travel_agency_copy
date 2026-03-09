using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Users;
using Travel_agency.Core.Enums;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.BLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJWTProvider _jwtProvider;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJWTProvider jwtProvider, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
        }

        public async Task<UserModel> Register(RegisterUserModel registerModel)
        {
            if (registerModel == null)
                throw new ArgumentNullException(nameof(registerModel), "Register data cannot be null");

            var existingByEmail = await _unitOfWork.Users.GetUserByEmailAsync(registerModel.Email);
            if (existingByEmail != null)
                throw new ConflictException("Email is already in use");

            if (!IsPasswordStrong(registerModel.Password))
                throw new ValidationException("Password must be at least 8 characters long and include uppercase, lowercase, digit, and special character");

            var passwordHash = _passwordHasher.GenerateHash(registerModel.Password);

            var userEntity = new UserEntity
            {
                Id = Guid.NewGuid(),
                Username = registerModel.Username,
                Email = registerModel.Email,
                PasswordHash = passwordHash,
                Role = UserRole.Registered
            };

            var addedUser = await _unitOfWork.Users.AddUserAsync(userEntity);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<UserModel>(addedUser);
        }

        public async Task<(string, UserModel)> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                throw new BusinessValidationException("Email and password must not be empty");

            var userEntity = await _unitOfWork.Users.GetUserByEmailAsync(email);

            if (userEntity == null)
                throw new NotFoundException("User not found");

            if (!_passwordHasher.VerifyHash(password, userEntity.PasswordHash))
                throw new UnauthorizedAccessException("Invalid password");

            var userModel = _mapper.Map<UserModel>(userEntity);
            var token = _jwtProvider.GenerateToken(userModel);
            return (token, userModel);
        }

        private static bool IsPasswordStrong(string password)
        {
            var pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,}$";
            return Regex.IsMatch(password, pattern);
        }
    }
}
