using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoMapper;
using NSubstitute;
using Travel_agency.BLL.Services;
using Travel_agency.Core.BusinessModels.Hotels;
using Travel_agency.Core.Enums;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.Tests.ServicesTests;
public class HotelBookingServiceTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly HotelBookingService _service;

    public HotelBookingServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _unitOfWork = _fixture.Freeze<IUnitOfWork>();
        _mapper = _fixture.Freeze<IMapper>();
        _service = new HotelBookingService(_unitOfWork, _mapper);
    }

    [Fact]
    public async Task GetAllHotelBookingsAsync_ReturnsMappedModels()
    {
        var entities = _fixture.CreateMany<HotelBookingEntity>(3).ToList();
        var model = _fixture.CreateMany<HotelBookingModel>(3);
        _unitOfWork.HotelBookings.GetAllHotelBookingsAsync().Returns(entities);
        _mapper.Map<IEnumerable<HotelBookingModel>>(entities).Returns(model);

        var result = await _service.GetAllHotelBookingsAsync();

        Assert.Equal(model, result);
    }

    [Fact]
    public async Task GetHotelBookingByIdAsync_ExistingId_ReturnsDetailsModel()
    {
        var id = Guid.NewGuid();
        var entity = _fixture.Create<HotelBookingEntity>();
        var model = _fixture.Create<HotelBookingDetailsModel>();
        _unitOfWork.HotelBookings.GetHotelBookingByIdAsync(id).Returns(entity);
        _mapper.Map<HotelBookingDetailsModel>(entity).Returns(model);

        var result = await _service.GetHotelBookingByIdAsync(id);

        Assert.Equal(model, result);
    }

    [Fact]
    public async Task GetHotelBookingByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _unitOfWork.HotelBookings.GetHotelBookingByIdAsync(id).Returns(null as HotelBookingEntity);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetHotelBookingByIdAsync(id));
    }

    [Fact]
    public async Task AddHotelBookingAsync_ValidModel_ReturnsMappedModel()
    {
        var model = _fixture.Create<HotelBookingModel>();
        model.StartDate = DateTime.UtcNow.AddDays(1);
        model.EndDate = model.StartDate.AddDays(2);
        model.HotelRoomId = Guid.NewGuid();
        model.UserId = Guid.NewGuid();

        var entity = _fixture.Create<HotelBookingEntity>();
        var addedEntity = _fixture.Create<HotelBookingEntity>();
        var resultModel = _fixture.Create<HotelBookingModel>();

        _mapper.Map<HotelBookingEntity>(model).Returns(entity);
        _unitOfWork.HotelBookings.AddHotelBookingAsync(entity).Returns(addedEntity);
        _mapper.Map<HotelBookingModel>(addedEntity).Returns(resultModel);

        var result = await _service.AddHotelBookingAsync(model);

        Assert.Equal(resultModel, result);
    }

    [Theory]
    [InlineData(null)]
    public async Task AddHotelBookingAsync_NullModel_ThrowsArgumentNullException(HotelBookingModel model)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddHotelBookingAsync(model));
    }

    [Theory]
    [InlineData("UserId")]
    [InlineData("HotelRoomId")]
    [InlineData("StartDate")]
    [InlineData("EndDate")]
    [InlineData("NumberOfGuests")]
    [InlineData("Status")]
    public async Task AddHotelBookingAsync_InvalidModel_ThrowsBusinessValidationException(string field)
    {
        var model = _fixture.Build<HotelBookingModel>()
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.HotelRoomId, Guid.NewGuid())
            .With(x => x.StartDate, DateTime.UtcNow.AddDays(1))
            .With(x => x.EndDate, DateTime.UtcNow.AddDays(3))
            .With(x => x.NumberOfGuests, 2)
            .With(x => x.Status, Status.Pending)
            .Create();

        switch (field)
        {
            case "UserId": model.UserId = Guid.Empty; break;
            case "HotelRoomId": model.HotelRoomId = Guid.Empty; break;
            case "StartDate": model.StartDate = DateTime.UtcNow.AddDays(-1); break;
            case "EndDate": model.EndDate = model.StartDate; break;
            case "NumberOfGuests": model.NumberOfGuests = 0; break;
            case "Status": model.Status = (Status)999; break;
        }

        await Assert.ThrowsAsync<BusinessValidationException>(() => _service.AddHotelBookingAsync(model));
    }

    [Fact]
    public async Task UpdateHotelBookingAsync_NotFound_ThrowsNotFoundException()
    {
        var model = _fixture.Create<HotelBookingModel>();
        model.StartDate = DateTime.UtcNow.AddDays(1);
        model.EndDate = model.StartDate.AddDays(2);
        model.HotelRoomId = Guid.NewGuid();
        model.UserId = Guid.NewGuid();

        var entity = _fixture.Create<HotelBookingEntity>();
        _mapper.Map<HotelBookingEntity>(model).Returns(entity);
        _unitOfWork.HotelBookings.UpdateHotelBookingAsync(entity).Returns(null as HotelBookingEntity);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateHotelBookingAsync(model));
    }

    [Fact]
    public async Task UpdateHotelBookingAsync_ValidModel_ReturnsMappedModel()
    {
        var model = _fixture.Create<HotelBookingModel>();
        model.StartDate = DateTime.UtcNow.AddDays(1);
        model.EndDate = model.StartDate.AddDays(2);
        model.HotelRoomId = Guid.NewGuid();
        model.UserId = Guid.NewGuid();

        var entity = _fixture.Create<HotelBookingEntity>();
        var updatedEntity = _fixture.Create<HotelBookingEntity>();
        var resultModel = _fixture.Create<HotelBookingModel>();

        _mapper.Map<HotelBookingEntity>(model).Returns(entity);
        _unitOfWork.HotelBookings.UpdateHotelBookingAsync(entity).Returns(updatedEntity);
        _mapper.Map<HotelBookingModel>(updatedEntity).Returns(resultModel);

        var result = await _service.UpdateHotelBookingAsync(model);

        Assert.Equal(resultModel, result);
    }

    [Fact]
    public async Task DeleteHotelBookingAsync_ExistingId_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var entity = _fixture.Create<HotelBookingEntity>();
        _unitOfWork.HotelBookings.GetHotelBookingByIdAsync(id).Returns(entity);

        var result = await _service.DeleteHotelBookingAsync(id);

        Assert.True(result);
        await _unitOfWork.HotelBookings.Received(1).DeleteHotelBookingAsync(id);
    }

    [Fact]
    public async Task DeleteHotelBookingAsync_NonExistingId_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _unitOfWork.HotelBookings.GetHotelBookingByIdAsync(id).Returns(null as HotelBookingEntity);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteHotelBookingAsync(id));
    }
}