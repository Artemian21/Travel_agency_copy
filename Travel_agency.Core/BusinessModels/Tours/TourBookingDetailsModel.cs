using Travel_agency.Core.BusinessModels.Users;

namespace Travel_agency.Core.BusinessModels.Tours
{
    public class TourBookingDetailsModel : TourBookingModel
    {
        public required TourModel Tour { get; set; }
        public required UserModel User { get; set; }
    }
}
