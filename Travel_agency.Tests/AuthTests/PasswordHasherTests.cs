using AutoFixture;
using Travel_agency.BLL.Abstractions;
using Travel_agency.BLL.Auth;
namespace Travel_agency.Tests.AuthTests;

public class PasswordHasherTests
{
    private readonly PasswordHasher _passwordHasher;
    private readonly IFixture _fixture;

    public PasswordHasherTests()
    {
        _fixture = new Fixture();
        _passwordHasher = new PasswordHasher();
    }

    [Fact]
    public void GenerateHash_ShouldReturnNonNullNonEmptyString()
    {
        var password = _fixture.Create<string>();
        var hash = _passwordHasher.GenerateHash(password);
        Assert.False(string.IsNullOrEmpty(hash));
    }

    [Fact]
    public void GenerateHash_SamePassword_ShouldGenerateDifferentHashes()
    {
        var password = _fixture.Create<string>();
        var hash1 = _passwordHasher.GenerateHash(password);
        var hash2 = _passwordHasher.GenerateHash(password);
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void VerifyHash_ShouldReturnTrue_ForCorrectPasswordAndHash()
    {
        var password = _fixture.Create<string>();
        var hash = _passwordHasher.GenerateHash(password);
        var result = _passwordHasher.VerifyHash(password, hash);
        Assert.True(result);
    }

    [Fact]
    public void VerifyHash_ShouldReturnFalse_ForIncorrectPassword()
    {
        var password = _fixture.Create<string>();
        var hash = _passwordHasher.GenerateHash(password);
        var wrongPassword = password + "wrong";
        var result = _passwordHasher.VerifyHash(wrongPassword, hash);
        Assert.False(result);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(null, "somehash")]
    [InlineData("password", null)]
    public void VerifyHash_ShouldThrowArgumentNullException_ForNullOrEmptyInputs(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
        {
            Assert.Throws<ArgumentNullException>(() => _passwordHasher.VerifyHash(password, hash));
        }
        else
        {
            var result = _passwordHasher.VerifyHash(password, hash);
            Assert.False(result);
        }
    }
}
