namespace Travel_agency.Core.BusinessModels.Hotels
{
    public class HotelWithBookingsModel : HotelModel
    {
        public List<HotelRoomModel> HotelRooms { get; set; } = new List<HotelRoomModel>();
    }
}
