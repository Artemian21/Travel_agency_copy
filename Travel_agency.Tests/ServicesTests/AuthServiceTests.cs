using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoMapper;
using NSubstitute;
using System.ComponentModel.DataAnnotations;
using Travel_agency.BLL.Abstractions;
using Travel_agency.BLL.Services;
using Travel_agency.Core.BusinessModels.Users;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.Tests.ServicesTests;

public class AuthServiceTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJWTProvider _jwtProvider;
    private readonly IMapper _mapper;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _unitOfWork = _fixture.Freeze<IUnitOfWork>();
        _passwordHasher = _fixture.Freeze<IPasswordHasher>();
        _jwtProvider = _fixture.Freeze<IJWTProvider>();
        _mapper = _fixture.Freeze<IMapper>();

        _authService = new AuthService(_unitOfWork, _passwordHasher, _jwtProvider, _mapper);
    }

    [Fact]
    public async Task Register_ThrowsArgumentNullException_WhenInputIsNull()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _authService.Register(null));
    }

    [Fact]
    public async Task Register_ThrowsConflictException_WhenEmailAlreadyExists()
    {
        // Arrange
        var model = _fixture.Create<RegisterUserModel>();
        _unitOfWork.Users.GetUserByEmailAsync(model.Email).Returns(new UserEntity());

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(() => _authService.Register(model));
    }

    [Fact]
    public async Task Register_ThrowsValidationException_WhenPasswordIsWeak()
    {
        // Arrange
        var model = _fixture.Build<RegisterUserModel>()
                          .With(x => x.Password, "weak")
                          .Create();
        _unitOfWork.Users.GetUserByEmailAsync(model.Email).Returns(null as UserEntity);

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _authService.Register(model));
    }

    [Fact]
    public async Task Register_ReturnsUserModel_WhenSuccessful()
    {
        // Arrange
        var model = _fixture.Build<RegisterUserModel>()
                          .With(x => x.Password, "StrongPass1!")
                          .Create();
        _unitOfWork.Users.GetUserByEmailAsync(model.Email).Returns(null as UserEntity);

        var userEntity = _fixture.Build<UserEntity>()
                                 .With(x => x.Email, model.Email)
                                 .With(x => x.Username, model.Username)
                                 .Create();

        _passwordHasher.GenerateHash(model.Password).Returns("hashedPassword");
        _unitOfWork.Users.AddUserAsync(Arg.Any<UserEntity>()).Returns(userEntity);
        var userModel = _fixture.Create<UserModel>();
        _mapper.Map<UserModel>(userEntity).Returns(userModel);

        // Act
        var result = await _authService.Register(model);

        // Assert
        Assert.Equal(userModel, result);
    }

    [Fact]
    public async Task Login_ThrowsBusinessValidationException_WhenEmailOrPasswordIsEmpty()
    {
        await Assert.ThrowsAsync<BusinessValidationException>(() => _authService.Login(null, "pass"));
        await Assert.ThrowsAsync<BusinessValidationException>(() => _authService.Login("", "pass"));
        await Assert.ThrowsAsync<BusinessValidationException>(() => _authService.Login("email", null));
        await Assert.ThrowsAsync<BusinessValidationException>(() => _authService.Login("email", ""));
    }

    [Fact]
    public async Task Login_ThrowsNotFoundException_WhenUserNotFound()
    {
        // Arrange
        _unitOfWork.Users.GetUserByEmailAsync(Arg.Any<string>()).Returns(null as UserEntity);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => _authService.Login("test@example.com", "Password1!"));
    }

    [Fact]
    public async Task Login_ThrowsUnauthorizedAccessException_WhenPasswordInvalid()
    {
        // Arrange
        var user = _fixture.Create<UserEntity>();
        _unitOfWork.Users.GetUserByEmailAsync(user.Email).Returns(user);
        _passwordHasher.VerifyHash(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _authService.Login(user.Email, "wrongPass"));
    }

    [Fact]
    public async Task Login_ReturnsToken_WhenSuccessful()
    {
        // Arrange
        var user = _fixture.Create<UserEntity>();
        var userModel = _fixture.Create<UserModel>();
        var token = _fixture.Create<string>();

        _unitOfWork.Users.GetUserByEmailAsync(user.Email).Returns(user);
        _passwordHasher.VerifyHash(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
        _mapper.Map<UserModel>(user).Returns(userModel);
        _jwtProvider.GenerateToken(userModel).Returns(token);

        // Act
        var (actualToken, actualUser) = await _authService.Login(user.Email, "ValidPass1!");

        // Assert
        Assert.Equal(token, actualToken);
        Assert.Equal(userModel, actualUser);
    }
}