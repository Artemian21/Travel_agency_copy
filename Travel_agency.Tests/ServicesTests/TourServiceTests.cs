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

public class TourServiceTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly TourService _service;

    public TourServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _unitOfWork = _fixture.Freeze<IUnitOfWork>();
        _mapper = _fixture.Freeze<IMapper>();
        _service = new TourService(_unitOfWork, _mapper);
    }

    [Fact]
    public async Task GetAllToursAsync_ReturnsMappedModels()
    {
        var entities = _fixture.CreateMany<TourEntity>();
        var models = _fixture.CreateMany<TourModel>();
        _unitOfWork.Tours.GetAllToursAsync().Returns(entities);
        _mapper.Map<IEnumerable<TourModel>>(entities).Returns(models);

        var result = await _service.GetAllToursAsync();

        Assert.Equal(models, result);
    }

    [Fact]
    public async Task GetPagedToursAsync_ReturnsPagedResult()
    {
        var entities = _fixture.CreateMany<TourEntity>();
        var models = _fixture.CreateMany<TourModel>();
        _unitOfWork.Tours.GetTotalToursCountAsync().Returns(100);
        _unitOfWork.Tours.GetToursPagedAsync(1, 10).Returns(entities);
        _mapper.Map<IEnumerable<TourModel>>(entities).Returns(models);

        var result = await _service.GetPagedToursAsync(1, 10);

        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(100, result.TotalCount);
        Assert.Equal(models, result.Items);
    }

    [Fact]
    public async Task GetTourByIdAsync_ValidId_ReturnsModel()
    {
        var id = Guid.NewGuid();
        var entity = _fixture.Create<TourEntity>();
        var model = _fixture.Create<TourWithBookingsModel>();
        _unitOfWork.Tours.GetTourByIdAsync(id).Returns(entity);
        _mapper.Map<TourWithBookingsModel>(entity).Returns(model);

        var result = await _service.GetTourByIdAsync(id);

        Assert.Equal(model, result);
    }

    [Fact]
    public async Task GetTourByIdAsync_NotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _unitOfWork.Tours.GetTourByIdAsync(id).Returns(null as TourEntity);
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetTourByIdAsync(id));
    }

    [Fact]
    public async Task AddTourAsync_ValidTour_ReturnsModel()
    {
        var model = _fixture.Build<TourModel>()
            .With(x => x.Name, "Tour")
            .With(x => x.Country, "Country")
            .With(x => x.Region, "Region")
            .With(x => x.Type, TypeTour.Cruise)
            .With(x => x.Price, 100)
            .With(x => x.StartDate, DateTime.UtcNow.AddDays(1))
            .With(x => x.EndDate, DateTime.UtcNow.AddDays(2))
            .Create();

        var entity = _fixture.Create<TourEntity>();
        var savedEntity = _fixture.Create<TourEntity>();
        var resultModel = _fixture.Create<TourModel>();

        _mapper.Map<TourEntity>(model).Returns(entity);
        _unitOfWork.Tours.AddTourAsync(entity).Returns(savedEntity);
        _mapper.Map<TourModel>(savedEntity).Returns(resultModel);

        var result = await _service.AddTourAsync(model);

        Assert.Equal(resultModel, result);
    }

    [Theory]
    [InlineData("", "Country", "Region", 100, 1, 2, TypeTour.Cruise)]
    [InlineData("Tour", "", "Region", 100, 1, 2, TypeTour.Cruise)]
    [InlineData("Tour", "Country", "", 100, 1, 2, TypeTour.Cruise)]
    [InlineData("Tour", "Country", "Region", -10, 1, 2, TypeTour.Cruise)]
    [InlineData("Tour", "Country", "Region", 100, -1, 2, TypeTour.Cruise)]
    [InlineData("Tour", "Country", "Region", 100, 2, 1, TypeTour.Cruise)]
    public async Task AddTourAsync_InvalidModel_ThrowsValidation(string name, string country, string region, decimal price, int startOffset, int endOffset, TypeTour type)
    {
        var model = new TourModel
        {
            Name = name,
            Country = country,
            Region = region,
            Price = price,
            StartDate = DateTime.UtcNow.AddDays(startOffset),
            EndDate = DateTime.UtcNow.AddDays(endOffset),
            Type = type
        };

        await Assert.ThrowsAnyAsync<BusinessValidationException>(() => _service.AddTourAsync(model));
    }

    [Fact]
    public async Task UpdateTourAsync_ValidTour_ReturnsModel()
    {
        var model = _fixture.Build<TourModel>()
            .With(x => x.Name, "Tour")
            .With(x => x.Country, "Country")
            .With(x => x.Region, "Region")
            .With(x => x.Type, TypeTour.Cruise)
            .With(x => x.Price, 100)
            .With(x => x.StartDate, DateTime.UtcNow.AddDays(1))
            .With(x => x.EndDate, DateTime.UtcNow.AddDays(2))
            .Create();

        var entity = _fixture.Create<TourEntity>();
        var updatedEntity = _fixture.Create<TourEntity>();
        var resultModel = _fixture.Create<TourModel>();

        _mapper.Map<TourEntity>(model).Returns(entity);
        _unitOfWork.Tours.UpdateTourAsync(entity).Returns(updatedEntity);
        _mapper.Map<TourModel>(updatedEntity).Returns(resultModel);

        var result = await _service.UpdateTourAsync(model);

        Assert.Equal(resultModel, result);
    }

    [Fact]
    public async Task UpdateTourAsync_NotFound_ThrowsException()
    {
        var model = _fixture.Build<TourModel>()
            .With(x => x.Name, "Tour")
            .With(x => x.Country, "Country")
            .With(x => x.Region, "Region")
            .With(x => x.Type, TypeTour.Cruise)
            .With(x => x.Price, 100)
            .With(x => x.StartDate, DateTime.UtcNow.AddDays(1))
            .With(x => x.EndDate, DateTime.UtcNow.AddDays(2))
            .Create();

        var entity = _fixture.Create<TourEntity>();
        _mapper.Map<TourEntity>(model).Returns(entity);
        _unitOfWork.Tours.UpdateTourAsync(entity).Returns(null as TourEntity);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateTourAsync(model));
    }

    [Fact]
    public async Task DeleteTourAsync_ValidId_ReturnsTrue()
    {
        var id = Guid.NewGuid();
        var entity = _fixture.Create<TourEntity>();
        _unitOfWork.Tours.GetTourByIdAsync(id).Returns(entity);

        var result = await _service.DeleteTourAsync(id);

        Assert.True(result);
        await _unitOfWork.Tours.Received(1).DeleteTourAsync(id);
    }

    [Fact]
    public async Task DeleteTourAsync_NotFound_ThrowsNotFoundException()
    {
        var id = Guid.NewGuid();
        _unitOfWork.Tours.GetTourByIdAsync(id).Returns(null as TourEntity);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteTourAsync(id));
    }
}