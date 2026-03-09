namespace Travel_agency.Core.BusinessModels.Hotels
{
    public class HotelRoomWithBookingModel : HotelRoomModel
    {
        public required HotelModel Hotel { get; set; }
        public ICollection<HotelBookingModel> HotelBookings { get; set; } = new List<HotelBookingModel>();
    }
}
