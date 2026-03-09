
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

public class HotelRoomServiceTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly HotelRoomService _service;

    public HotelRoomServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _unitOfWork = _fixture.Freeze<IUnitOfWork>();
        _mapper = _fixture.Freeze<IMapper>();
        _service = new HotelRoomService(_unitOfWork, _mapper);
    }

    [Fact]
    public async Task GetAllHotelRoomsAsync_ReturnsMappedRooms()
    {
        var rooms = _fixture.CreateMany<HotelRoomEntity>(3);
        var model = _fixture.CreateMany<HotelRoomModel>(3);

        _unitOfWork.HotelRooms.GetAllHotelRoomsAsync().Returns(Task.FromResult(rooms));
        _mapper.Map<IEnumerable<HotelRoomModel>>(rooms).Returns(model);

        var result = await _service.GetAllHotelRoomsAsync();

        Assert.Equal(model, result);
    }

    [Fact]
    public async Task GetHotelRoomByIdAsync_ReturnsMappedRoom()
    {
        var id = Guid.NewGuid();
        var entity = _fixture.Create<HotelRoomEntity>();
        var model = _fixture.Create<HotelRoomWithBookingModel>();

        _unitOfWork.HotelRooms.GetHotelRoomByIdAsync(id).Returns(Task.FromResult(entity));
        _mapper.Map<HotelRoomWithBookingModel>(entity).Returns(model);

        var result = await _service.GetHotelRoomByIdAsync(id);

        Assert.Equal(model, result);
    }

    [Fact]
    public async Task GetHotelRoomByIdAsync_ThrowsIfNotFound()
    {
        var id = Guid.NewGuid();
        _unitOfWork.HotelRooms.GetHotelRoomByIdAsync(id).Returns((HotelRoomEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetHotelRoomByIdAsync(id));
    }

    [Fact]
    public async Task GetHotelRoomsByHotelIdAsync_ReturnsMappedRooms()
    {
        var id = Guid.NewGuid();
        var hotel = _fixture.Create<HotelEntity>();
        var rooms = _fixture.CreateMany<HotelRoomEntity>(3);
        var model = _fixture.CreateMany<HotelRoomModel>(3);

        _unitOfWork.Hotels.GetHotelByIdAsync(id).Returns(Task.FromResult(hotel));
        _unitOfWork.HotelRooms.GetRoomsByHotelIdAsync(id).Returns(Task.FromResult(rooms));
        _mapper.Map<IEnumerable<HotelRoomModel>>(rooms).Returns(model);

        var result = await _service.GetHotelRoomsByHotelIdAsync(id);

        Assert.Equal(model, result);
    }

    [Fact]
    public async Task GetHotelRoomsByHotelIdAsync_ThrowsIfHotelNotFound()
    {
        var id = Guid.NewGuid();
        _unitOfWork.Hotels.GetHotelByIdAsync(id).Returns((HotelEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetHotelRoomsByHotelIdAsync(id));
    }

    [Fact]
    public async Task AddHotelRoomAsync_ReturnsMappedResult()
    {
        var model = _fixture.Create<HotelRoomModel>();
        var entity = _fixture.Create<HotelRoomEntity>();
        var addedEntity = _fixture.Create<HotelRoomEntity>();
        var resultModel = _fixture.Create<HotelRoomModel>();

        _mapper.Map<HotelRoomEntity>(model).Returns(entity);
        _unitOfWork.HotelRooms.AddHotelRoomAsync(entity).Returns(addedEntity);
        _mapper.Map<HotelRoomModel>(addedEntity).Returns(resultModel);

        var result = await _service.AddHotelRoomAsync(model);

        Assert.Equal(resultModel, result);
    }

    [Theory]
    [InlineData(null)]
    public async Task AddHotelRoomAsync_ThrowsIfModelInvalid(HotelRoomModel? model)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddHotelRoomAsync(model!));
    }

    [Fact]
    public async Task AddHotelRoomAsync_ThrowsIfInvalidValues()
    {
        var model = new HotelRoomModel
        {
            Capacity = 0,
            PricePerNight = -1,
            HotelId = Guid.Empty,
            RoomType = (RoomType)999
        };

        await Assert.ThrowsAsync<BusinessValidationException>(() => _service.AddHotelRoomAsync(model));
    }

    [Fact]
    public async Task UpdateHotelRoomAsync_ReturnsMappedResult()
    {
        var model = _fixture.Create<HotelRoomModel>();
        var entity = _fixture.Create<HotelRoomEntity>();
        var updated = _fixture.Create<HotelRoomEntity>();
        var resultModel = _fixture.Create<HotelRoomModel>();

        _mapper.Map<HotelRoomEntity>(model).Returns(entity);
        _unitOfWork.HotelRooms.UpdateHotelRoomAsync(entity).Returns(updated);
        _mapper.Map<HotelRoomModel>(updated).Returns(resultModel);

        var result = await _service.UpdateHotelRoomAsync(model);

        Assert.Equal(resultModel, result);
    }

    [Fact]
    public async Task UpdateHotelRoomAsync_ThrowsIfNotFound()
    {
        var model = _fixture.Create<HotelRoomModel>();
        var entity = _fixture.Create<HotelRoomEntity>();

        _mapper.Map<HotelRoomEntity>(model).Returns(entity);
        _unitOfWork.HotelRooms.UpdateHotelRoomAsync(entity).Returns((HotelRoomEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateHotelRoomAsync(model));
    }

    [Fact]
    public async Task DeleteHotelRoomAsync_ReturnsTrueIfDeleted()
    {
        var id = Guid.NewGuid();
        var entity = _fixture.Create<HotelRoomEntity>();

        _unitOfWork.HotelRooms.GetHotelRoomByIdAsync(id).Returns(Task.FromResult(entity));

        var result = await _service.DeleteHotelRoomAsync(id);

        Assert.True(result);
        await _unitOfWork.HotelRooms.Received(1).DeleteHotelRoomAsync(id);
    }

    [Fact]
    public async Task DeleteHotelRoomAsync_ThrowsIfNotFound()
    {
        var id = Guid.NewGuid();
        _unitOfWork.HotelRooms.GetHotelRoomByIdAsync(id).Returns((HotelRoomEntity?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteHotelRoomAsync(id));
    }
}