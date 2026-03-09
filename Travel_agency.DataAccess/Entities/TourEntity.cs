using Travel_agency.Core.Enums;

namespace Travel_agency.DataAccess.Entities
{
    public class TourEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public TypeTour Type { get; set; }
        public required string Country { get; set; }
        public required string Region { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }

        public ICollection<TourBookingEntity> TourBookings { get; set; } = new List<TourBookingEntity>();

    }
}
