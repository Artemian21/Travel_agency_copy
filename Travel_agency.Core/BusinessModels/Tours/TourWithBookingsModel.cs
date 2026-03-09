namespace Travel_agency.Core.BusinessModels.Tours
{
    public class TourWithBookingsModel : TourModel
    {
        public ICollection<TourBookingModel> TourBookings { get; set; } = new List<TourBookingModel>();
    }
}
