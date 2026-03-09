using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoMapper;
using NSubstitute;
using Travel_agency.BLL.Services;
using Travel_agency.Core.BusinessModels.Hotels;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.Tests.ServicesTests;

public class HotelServiceTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IMapper _mapperMock;
    private readonly HotelService _hotelService;

    public HotelServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _unitOfWorkMock = _fixture.Freeze<IUnitOfWork>();
        _mapperMock = _fixture.Freeze<IMapper>();

        _hotelService = new HotelService(_unitOfWorkMock, _mapperMock);
    }

    [Fact]
    public async Task GetAllHotelsAsync_ReturnsMappedModels()
    {
        // Arrange
        var hotelEntities = _fixture.CreateMany<HotelEntity>().ToList();
        var hotelModels = _fixture.CreateMany<HotelModel>().ToList();

        _unitOfWorkMock.Hotels.GetAllHotelsAsync().Returns(hotelEntities);
        _mapperMock.Map<IEnumerable<HotelModel>>(hotelEntities).Returns(hotelModels);

        // Act
        var result = await _hotelService.GetAllHotelsAsync();

        // Assert
        Assert.Equal(hotelModels, result);
    }

    [Fact]
    public async Task GetHotelByIdAsync_WhenFound_ReturnsMappedModel()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var hotelEntity = _fixture.Create<HotelEntity>();
        var hotelModel = _fixture.Create<HotelWithBookingsModel>();

        _unitOfWorkMock.Hotels.GetHotelByIdAsync(hotelId).Returns(hotelEntity);
        _mapperMock.Map<HotelWithBookingsModel>(hotelEntity).Returns(hotelModel);

        // Act
        var result = await _hotelService.GetHotelByIdAsync(hotelId);

        // Assert
        Assert.Equal(hotelModel, result);
    }

    [Fact]
    public async Task GetHotelByIdAsync_WhenNotFound_ThrowsNotFoundException()
    {
        var hotelId = Guid.NewGuid();
        _unitOfWorkMock.Hotels.GetHotelByIdAsync(hotelId).Returns(null as HotelEntity);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _hotelService.GetHotelByIdAsync(hotelId));
        Assert.Contains(hotelId.ToString(), ex.Message);
    }

    [Fact]
    public async Task AddHotelAsync_ValidHotel_AddsAndReturnsModel()
    {
        var hotelModel = _fixture.Build<HotelModel>()
            .With(h => h.Name, "TestHotel")
            .With(h => h.Country, "Ukraine")
            .With(h => h.City, "Kyiv")
            .With(h => h.Address, "TestStreet")
            .Create();

        var hotelEntity = _fixture.Create<HotelEntity>();
        var addedHotel = _fixture.Create<HotelEntity>();
        var resultModel = _fixture.Create<HotelModel>();

        _mapperMock.Map<HotelEntity>(hotelModel).Returns(hotelEntity);
        _unitOfWorkMock.Hotels.AddHotelAsync(hotelEntity).Returns(addedHotel);
        _mapperMock.Map<HotelModel>(addedHotel).Returns(resultModel);

        var result = await _hotelService.AddHotelAsync(hotelModel);

        Assert.Equal(resultModel, result);
    }

    [Theory]
    [InlineData(null, "Ukraine", "Kyiv", "TestStreet", "Hotel name is required.")]
    [InlineData("TestHotel", null, "Kyiv", "TestStreet", "Country is required.")]
    [InlineData("TestHotel", "Ukraine", null, "TestStreet", "City is required.")]
    [InlineData("TestHotel", "Ukraine", "Kyiv", null, "Address is required.")]
    public async Task AddHotelAsync_InvalidHotel_ThrowsValidation(string name, string country, string city, string address, string expectedMessage)
    {
        var hotelModel = new HotelModel
        {
            Name = name,
            Country = country,
            City = city,
            Address = address
        };

        var ex = await Assert.ThrowsAsync<BusinessValidationException>(() => _hotelService.AddHotelAsync(hotelModel));
        Assert.Equal(expectedMessage, ex.Message);
    }

    [Fact]
    public async Task AddHotelAsync_NullModel_ThrowsArgumentNullException()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _hotelService.AddHotelAsync(null));
    }

    [Fact]
    public async Task UpdateHotelAsync_WhenFound_UpdatesAndReturnsModel()
    {
        var hotelModel = _fixture.Build<HotelModel>()
            .With(h => h.Name, "TestHotel")
            .With(h => h.Country, "Ukraine")
            .With(h => h.City, "Kyiv")
            .With(h => h.Address, "TestStreet")
            .Create();

        var hotelEntity = _fixture.Create<HotelEntity>();
        var updatedHotel = _fixture.Create<HotelEntity>();
        var resultModel = _fixture.Create<HotelModel>();

        _mapperMock.Map<HotelEntity>(hotelModel).Returns(hotelEntity);
        _unitOfWorkMock.Hotels.UpdateHotelAsync(hotelEntity).Returns(updatedHotel);
        _mapperMock.Map<HotelModel>(updatedHotel).Returns(resultModel);

        var result = await _hotelService.UpdateHotelAsync(hotelModel);

        Assert.Equal(resultModel, result);
    }

    [Fact]
    public async Task UpdateHotelAsync_WhenNotFound_ThrowsNotFoundException()
    {
        var hotelModel = _fixture.Build<HotelModel>()
            .With(h => h.Name, "TestHotel")
            .With(h => h.Country, "Ukraine")
            .With(h => h.City, "Kyiv")
            .With(h => h.Address, "TestStreet")
            .Create();

        var hotelEntity = _fixture.Create<HotelEntity>();

        _mapperMock.Map<HotelEntity>(hotelModel).Returns(hotelEntity);
        _unitOfWorkMock.Hotels.UpdateHotelAsync(hotelEntity).Returns(null as HotelEntity);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _hotelService.UpdateHotelAsync(hotelModel));
        Assert.Contains(hotelModel.Id.ToString(), ex.Message);
    }

    [Fact]
    public async Task DeleteHotelAsync_WhenHotelExists_DeletesSuccessfully()
    {
        var hotelId = Guid.NewGuid();
        var hotelEntity = _fixture.Create<HotelEntity>();

        _unitOfWorkMock.Hotels.GetHotelByIdAsync(hotelId).Returns(hotelEntity);

        var result = await _hotelService.DeleteHotelAsync(hotelId);

        Assert.True(result);
        await _unitOfWorkMock.Hotels.Received(1).DeleteHotelAsync(hotelId);
    }

    [Fact]
    public async Task DeleteHotelAsync_WhenHotelNotFound_ThrowsNotFoundException()
    {
        var hotelId = Guid.NewGuid();

        _unitOfWorkMock.Hotels.GetHotelByIdAsync(hotelId).Returns(null as HotelEntity);

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _hotelService.DeleteHotelAsync(hotelId));
        Assert.Contains(hotelId.ToString(), ex.Message);
    }
}