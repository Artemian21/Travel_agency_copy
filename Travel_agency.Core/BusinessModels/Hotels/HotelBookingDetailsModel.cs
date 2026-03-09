using Travel_agency.Core.BusinessModels.Users;

namespace Travel_agency.Core.BusinessModels.Hotels
{
    public class HotelBookingDetailsModel : HotelBookingModel
    {
        public required HotelRoomModel HotelRoom { get; set; }
        public required UserModel User { get; set; }
    }
}
