using AutoFixture;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Travel_agency.BLL.Auth;
using Travel_agency.Core.BusinessModels.Users;
using Travel_agency.Core.Enums;

namespace Travel_agency.Tests.AuthTests;

public class JWTProviderTests
{
    private readonly IFixture _fixture;
    public JWTProviderTests()
    {
        _fixture = new Fixture();
    }

    private IConfiguration GetValidConfiguration()
    {
        var config = new Dictionary<string, string>
        {
            { "JWT:Key", Convert.ToBase64String(Encoding.UTF8.GetBytes("SuperSecretKey1234567890")) },
            { "JWT:Issuer", "TravelAgency" },
            { "JWT:Audience", "TravelAgencyUsers" },
            { "JWT:TokenExpirationInMinutes", "60" }
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(config)
            .Build();
    }

    [Fact]
    public void GenerateToken_ValidUser_ReturnsValidJwt()
    {
        // Arrange
        var user = _fixture.Build<UserModel>()
            .With(u => u.Role, UserRole.Administrator)
            .With(u => u.Email, "test@example.com")
            .With(u => u.Username, "TestUser")
            .Create();

        var configuration = GetValidConfiguration();
        var jwtProvider = new JWTProvider(configuration);

        // Act
        var token = jwtProvider.GenerateToken(user);

        // Assert
        Assert.NotNull(token);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal("TravelAgency", jwtToken.Issuer);
        Assert.Equal("TravelAgencyUsers", jwtToken.Audiences.First());

        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Name && c.Value == user.Username);
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Email && c.Value == user.Email);
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
        Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == user.Role.ToString());
    }

    [Theory]
    [InlineData("JWT:Key")]
    [InlineData("JWT:Issuer")]
    [InlineData("JWT:Audience")]
    [InlineData("JWT:TokenExpirationInMinutes")]
    public void GenerateToken_MissingConfiguration_ThrowsException(string missingKey)
    {
        // Arrange
        var configDict = new Dictionary<string, string>
        {
            { "JWT:Key", "SuperSecretKey1234567890" }, // тут можна залишити у відкритому вигляді, Base64 не обов'язково
            { "JWT:Issuer", "TravelAgency" },
            { "JWT:Audience", "TravelAgencyUsers" },
            { "JWT:TokenExpirationInMinutes", "60" }
        };

        configDict.Remove(missingKey);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        var user = _fixture.Build<UserModel>()
            .With(u => u.Role, UserRole.Administrator)
            .Create();

        var jwtProvider = new JWTProvider(configuration);

        // Act & Assert
        var ex = Assert.ThrowsAny<Exception>(() => jwtProvider.GenerateToken(user));
        Assert.NotNull(ex);
    }


    [Fact]
    public void GenerateToken_InvalidExpiration_ThrowsFormatException()
    {
        // Arrange
        var configDict = new Dictionary<string, string>
        {
            { "JWT:Key", Convert.ToBase64String(Encoding.UTF8.GetBytes("SecretKey123456")) },
            { "JWT:Issuer", "TravelAgency" },
            { "JWT:Audience", "TravelAgencyUsers" },
            { "JWT:TokenExpirationInMinutes", "invalid" } // <-- не число
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configDict)
            .Build();

        var user = _fixture.Build<UserModel>().Create();
        var jwtProvider = new JWTProvider(configuration);

        // Act + Assert
        Assert.Throws<FormatException>(() => jwtProvider.GenerateToken(user));
    }
}