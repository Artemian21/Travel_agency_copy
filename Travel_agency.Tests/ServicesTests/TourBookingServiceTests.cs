using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoMapper;
using NSubstitute;
using Travel_agency.BLL.Services;
using Travel_agency.Core.BusinessModels.Tours;
using Travel_agency.Core.Enums;
using Travel_agency.Core.Exceptions;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.Tests.ServicesTests;

public class TourBookingServiceTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TourBookingService _service;

    public TourBookingServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _fixture.Behaviors.Clear();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _unitOfWork = _fixture.Freeze<IUnitOfWork>();
        _mapper = _fixture.Freeze<IMapper>();
        _service = new TourBookingService(_unitOfWork, _mapper);
    }

    [Fact]
    public async Task GetAllTourBookingsAsync_ReturnsMappedModels()
    {
        var entities = _fixture.CreateMany<TourBookingEntity>();
        var models = _fixture.CreateMany<TourBookingModel>();

        _unitOfWork.TourBookings.GetAllTourBookingsAsync().Returns(entities);
        _mapper.Map<IEnumerable<TourBookingModel>>(entities).Returns(models);

        var result = await _service.GetAllTourBookingsAsync();

        Assert.Equal(models, result);
    }

    [Fact]
    public async Task GetTourBookingByIdAsync_ValidId_ReturnsModel()
    {
        var id = Guid.NewGuid();
        var entity = _fixture.Create<TourBookingEntity>();
        var model = _fixture.Create<TourBookingDetailsModel>();

        _unitOfWork.TourBookings.GetTourBookingByIdAsync(id).Returns(entity);
        _mapper.Map<TourBookingDetailsModel>(entity).Returns(model);

        var result = await _service.GetTourBookingByIdAsync(id);

        Assert.Equal(model, result);
    }

    [Fact]
    public async Task GetTourBookingByIdAsync_NotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _unitOfWork.TourBookings.GetTourBookingByIdAsync(id).Returns(null as TourBookingEntity);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetTourBookingByIdAsync(id));
    }

    [Fact]
    public async Task AddTourBookingAsync_ValidModel_ReturnsMappedModel()
    {
        var model = _fixture.Build<TourBookingModel>()
            .With(x => x.TourId, Guid.NewGuid())
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.Status, Status.Confirmed)
            .Create();

        var entity = _fixture.Create<TourBookingEntity>();
        var savedEntity = _fixture.Create<TourBookingEntity>();
        var resultModel = _fixture.Create<TourBookingModel>();

        _mapper.Map<TourBookingEntity>(model).Returns(entity);
        _unitOfWork.TourBookings.AddTourBookingAsync(entity).Returns(savedEntity);
        _mapper.Map<TourBookingModel>(savedEntity).Returns(resultModel);

        var result = await _service.AddTourBookingAsync(model);

        Assert.Equal(resultModel, result);
    }

    [Theory]
    [InlineData(null)]
    public async Task AddTourBookingAsync_NullModel_ThrowsArgumentNullException(TourBookingModel model)
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() => _service.AddTourBookingAsync(model));
    }

    [Fact]
    public async Task AddTourBookingAsync_EmptyTourId_ThrowsBusinessValidationException()
    {
        var model = _fixture.Build<TourBookingModel>()
            .With(x => x.TourId, Guid.Empty)
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.Status, Status.Confirmed)
            .Create();

        await Assert.ThrowsAsync<BusinessValidationException>(() => _service.AddTourBookingAsync(model));
    }

    [Fact]
    public async Task AddTourBookingAsync_InvalidStatus_ThrowsBusinessValidationException()
    {
        var model = _fixture.Build<TourBookingModel>()
            .With(x => x.TourId, Guid.NewGuid())
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.Status, (Status)999)
            .Create();

        await Assert.ThrowsAsync<BusinessValidationException>(() => _service.AddTourBookingAsync(model));
    }

    [Fact]
    public async Task UpdateTourBookingAsync_ValidModel_ReturnsMappedModel()
    {
        var model = _fixture.Build<TourBookingModel>()
            .With(x => x.TourId, Guid.NewGuid())
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.Status, Status.Confirmed)
            .Create();

        var entity = _fixture.Create<TourBookingEntity>();
        var updatedEntity = _fixture.Create<TourBookingEntity>();
        var resultModel = _fixture.Create<TourBookingModel>();

        _mapper.Map<TourBookingEntity>(model).Returns(entity);
        _unitOfWork.TourBookings.UpdateTourBookingAsync(entity).Returns(updatedEntity);
        _mapper.Map<TourBookingModel>(updatedEntity).Returns(resultModel);

        var result = await _service.UpdateTourBookingAsync(model);

        Assert.Equal(resultModel, result);
    }

    [Fact]
    public async Task UpdateTourBookingAsync_NotFound_ThrowsNotFoundException()
    {
        var model = _fixture.Build<TourBookingModel>()
            .With(x => x.TourId, Guid.NewGuid())
            .With(x => x.UserId, Guid.NewGuid())
            .With(x => x.Status, Status.Confirmed)
            .Create();

        var entity = _fixture.Create<TourBookingEntity>();
        _mapper.Map<TourBookingEntity>(model).Returns(entity);
        _unitOfWork.TourBookings.UpdateTourBookingAsync(entity).Returns(null as TourBookingEntity);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateTourBookingAsync(model));
    }

    [Fact]
    public async Task DeleteTourBookingAsync_ValidId_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var entity = _fixture.Create<TourBookingEntity>();
        _unitOfWork.TourBookings.GetTourBookingByIdAsync(id).Returns(entity);

        var result = await _service.DeleteTourBookingAsync(id);

        Assert.True(result);
        await _unitOfWork.TourBookings.Received(1).DeleteTourBookingAsync(id);
    }

    [Fact]
    public async Task DeleteTourBookingAsync_NotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _unitOfWork.TourBookings.GetTourBookingByIdAsync(id).Returns(null as TourBookingEntity);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteTourBookingAsync(id));
    }
}
