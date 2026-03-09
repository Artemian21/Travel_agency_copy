using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoMapper;
using NSubstitute;
using Travel_agency.BLL.Services;
using Travel_agency.Core.BusinessModels.Tours;
using Travel_agency.Core.Enums;
using Travel_agency.DataAccess.Abstraction;
using Travel_agency.DataAccess.Entities;

namespace Travel_agency.Tests.ServicesTests;

public class TourQueryServiceTests
{
    private readonly IFixture _fixture;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public TourQueryServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        _unitOfWork = _fixture.Freeze<IUnitOfWork>();

        // Мок репозиторію турів
        _tourRepository = Substitute.For<ITourRepository>();
        _unitOfWork.Tours.Returns(_tourRepository);

        // Налаштування AutoMapper для тестів
        var mapperConfig = new MapperConfiguration(cfg => cfg.CreateMap<TourEntity, TourModel>());
        _mapper = mapperConfig.CreateMapper();
    }

    private static List<TourEntity> CreateTestTours()
    {
        return new List<TourEntity>
        {
            new TourEntity { Name = "Beach Tour", Country = "Italy", Region = "South", Type = TypeTour.Adventure, StartDate = new DateTime(2024, 7, 1), Price = 1000 },
            new TourEntity { Name = "Mountain Hike", Country = "France", Region = "Alps", Type =  TypeTour.Family, StartDate = new DateTime(2024, 8, 10), Price = 1500 },
            new TourEntity { Name = "City Walk", Country = "Italy", Region = "Rome", Type =  TypeTour.Beach, StartDate = new DateTime(2024, 9, 5), Price = 800 },
        };
    }

    [Fact]
    public async Task GetFilteredToursAsync_ReturnsFilteredByCountry()
    {
        var testTours = CreateTestTours();
        _tourRepository.GetAllToursAsync().Returns(Task.FromResult<IEnumerable<TourEntity>>(testTours));

        var sut = new TourQueryService(_unitOfWork, _mapper);
        var filter = new TourFilterModel { Country = "italy" };

        // Act
        var result = await sut.GetFilteredToursAsync(filter);

        // Assert
        Assert.All(result, r => Assert.Equal("Italy", r.Country));
    }

    [Fact]
    public async Task GetFilteredToursAsync_ReturnsFilteredByType()
    {
        var testTours = CreateTestTours();
        _tourRepository.GetAllToursAsync().Returns(Task.FromResult<IEnumerable<TourEntity>>(testTours));

        var sut = new TourQueryService(_unitOfWork, _mapper);
        var filter = new TourFilterModel { Type = TypeTour.Adventure };

        var result = await sut.GetFilteredToursAsync(filter);

        Assert.Single(result);
        Assert.All(result, r => Assert.Equal(TypeTour.Adventure, r.Type));
    }

    [Fact]
    public async Task GetFilteredToursAsync_ReturnsFilteredByDateAndPrice()
    {
        var testTours = CreateTestTours();
        _tourRepository.GetAllToursAsync().Returns(Task.FromResult<IEnumerable<TourEntity>>(testTours));

        var sut = new TourQueryService(_unitOfWork, _mapper);
        var filter = new TourFilterModel
        {
            StartDateFrom = new DateTime(2024, 8, 1),
            StartDateTo = new DateTime(2024, 9, 10),
            Price = 1000
        };

        var result = await sut.GetFilteredToursAsync(filter);

        Assert.Single(result);
        Assert.All(result, r =>
        {
            Assert.True(r.StartDate >= filter.StartDateFrom);
            Assert.True(r.StartDate <= filter.StartDateTo);
            Assert.True(r.Price <= filter.Price);
        });
    }

    [Fact]
    public async Task SearchToursAsync_ReturnsMatchingByName()
    {
        var testTours = CreateTestTours();
        _tourRepository.GetAllToursAsync().Returns(Task.FromResult<IEnumerable<TourEntity>>(testTours));

        var sut = new TourQueryService(_unitOfWork, _mapper);

        var result = await sut.SearchToursAsync("city walk");

        Assert.Single(result);
        Assert.All(result, r => Assert.Contains("city walk", r.Name, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchToursAsync_ReturnsMatchingByDate()
    {
        var testTours = CreateTestTours();
        _tourRepository.GetAllToursAsync().Returns(Task.FromResult<IEnumerable<TourEntity>>(testTours));

        var sut = new TourQueryService(_unitOfWork, _mapper);

        var searchDate = "2024-08-10";

        var result = await sut.SearchToursAsync(searchDate);

        Assert.All(result, r => Assert.Equal(new DateTime(2024, 8, 10), r.StartDate.Date));
    }

    [Fact]
    public async Task SearchToursAsync_EmptyString_ReturnsAll()
    {
        var testTours = CreateTestTours();
        _tourRepository.GetAllToursAsync().Returns(Task.FromResult<IEnumerable<TourEntity>>(testTours));

        var sut = new TourQueryService(_unitOfWork, _mapper);
        var result = await sut.SearchToursAsync("");

        Assert.Equal(testTours.Count, result.Count());
    }
}