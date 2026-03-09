
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoMapper;
using NSubstitute;
using Travel_agency.BLL.Services;
using Travel_agency.Core.BusinessModels.Transports;
using Travel_agency.Core.Enums;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.Tests.ServicesTests;

public class TicketBookingServiceTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TicketBookingService _service;

    public TicketBookingServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _unitOfWork = _fixture.Freeze<IUnitOfWork>();
        _mapper = _fixture.Freeze<IMapper>();
        _service = new TicketBookingService(_unitOfWork, _mapper);
    }

    [Fact]
    public async Task GetAllTicketBookingsAsync_ReturnsMappedModels()
    {
        var entities = _fixture.CreateMany<TicketBookingEntity>(3);
        var models = _fixture.CreateMany<TicketBookingModel>(3);
        _unitOfWork.TicketBookings.GetAllTicketBookingsAsync().Returns(Task.FromResult(entities));
        _mapper.Map<IEnumerable<TicketBookingModel>>(entities).Returns(models);

        var result = await _service.GetAllTicketBookingsAsync();

        Assert.Equal(models, result);
    }

    [Fact]
    public async Task GetTicketBookingByIdAsync_ReturnsModel_WhenExists()
    {
        var entity = _fixture.Create<TicketBookingEntity>();
        var model = _fixture.Create<TicketBookingDetailsModel>();
        var id = Guid.NewGuid();

        _unitOfWork.TicketBookings.GetTicketBookingByIdAsync(id).Returns(Task.FromResult(entity));
        _mapper.Map<TicketBookingDetailsModel>(entity).Returns(model);

        var result = await _service.GetTicketBookingByIdAsync(id);

        Assert.Equal(model, result);
    }

    [Fact]
    public async Task GetTicketBookingByIdAsync_Throws_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _unitOfWork.TicketBookings.GetTicketBookingByIdAsync(id).Returns(Task.FromResult<TicketBookingEntity>(null));

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.GetTicketBookingByIdAsync(id));
        Assert.Equal($"Ticket Booking with ID {id} not found.", ex.Message);
    }

    [Fact]
    public async Task AddTicketBookingAsync_ValidInput_ReturnsMappedModel()
    {
        var model = _fixture.Build<TicketBookingModel>()
            .With(x => x.TransportId, Guid.NewGuid())
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.Status, Status.Confirmed)
            .Create();

        var entity = _fixture.Create<TicketBookingEntity>();
        var addedEntity = _fixture.Create<TicketBookingEntity>();
        var expectedModel = _fixture.Create<TicketBookingModel>();

        _mapper.Map<TicketBookingEntity>(model).Returns(entity);
        _unitOfWork.TicketBookings.AddTicketBookingAsync(entity).Returns(Task.FromResult(addedEntity));
        _mapper.Map<TicketBookingModel>(addedEntity).Returns(expectedModel);

        var result = await _service.AddTicketBookingAsync(model);

        Assert.Equal(expectedModel, result);
    }

    [Fact]
    public async Task AddTicketBookingAsync_Throws_WhenInvalid()
    {
        var model = new TicketBookingModel(); // Invalid model (empty GUIDs)

        await Assert.ThrowsAsync<BusinessValidationException>(() => _service.AddTicketBookingAsync(model));
    }

    [Fact]
    public async Task UpdateTicketBookingAsync_ValidInput_ReturnsUpdatedModel()
    {
        var model = _fixture.Build<TicketBookingModel>()
            .With(x => x.TransportId, Guid.NewGuid())
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.Status, Status.Confirmed)
            .Create();

        var entity = _fixture.Create<TicketBookingEntity>();
        var updatedEntity = _fixture.Create<TicketBookingEntity>();
        var expectedModel = _fixture.Create<TicketBookingModel>();

        _mapper.Map<TicketBookingEntity>(model).Returns(entity);
        _unitOfWork.TicketBookings.UpdateTicketBookingAsync(entity).Returns(Task.FromResult(updatedEntity));
        _mapper.Map<TicketBookingModel>(updatedEntity).Returns(expectedModel);

        var result = await _service.UpdateTicketBookingAsync(model);

        Assert.Equal(expectedModel, result);
    }

    [Fact]
    public async Task UpdateTicketBookingAsync_Throws_WhenNotFound()
    {
        var model = _fixture.Build<TicketBookingModel>()
            .With(x => x.TransportId, Guid.NewGuid())
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.Status, Status.Confirmed)
            .Create();

        var entity = _fixture.Create<TicketBookingEntity>();
        _mapper.Map<TicketBookingEntity>(model).Returns(entity);
        _unitOfWork.TicketBookings.UpdateTicketBookingAsync(entity).Returns(Task.FromResult<TicketBookingEntity>(null));

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateTicketBookingAsync(model));
        Assert.Equal($"Ticket Booking with ID {model.Id} not found.", ex.Message);
    }

    [Fact]
    public async Task DeleteTicketBookingAsync_ReturnsTrue_WhenDeleted()
    {
        var id = Guid.NewGuid();
        var entity = _fixture.Create<TicketBookingEntity>();
        _unitOfWork.TicketBookings.GetTicketBookingByIdAsync(id).Returns(Task.FromResult(entity));

        var result = await _service.DeleteTicketBookingAsync(id);

        await _unitOfWork.TicketBookings.Received(1).DeleteTicketBookingAsync(id);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteTicketBookingAsync_Throws_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _unitOfWork.TicketBookings.GetTicketBookingByIdAsync(id).Returns(Task.FromResult<TicketBookingEntity>(null));

        var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteTicketBookingAsync(id));
        Assert.Equal($"Ticket Booking with ID {id} not found.", ex.Message);
    }

    [Fact]
    public async Task AddTicketBookingAsync_Throws_WhenModelIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddTicketBookingAsync(null));
    }

    [Fact]
    public async Task UpdateTicketBookingAsync_Throws_WhenModelIsNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateTicketBookingAsync(null));
    }
}