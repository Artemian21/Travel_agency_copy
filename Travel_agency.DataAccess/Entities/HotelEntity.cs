namespace Travel_agency.DataAccess.Entities
{
    public class HotelEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Country { get; set; }
        public required string City { get; set; }
        public required string Address { get; set; }
        public ICollection<HotelRoomEntity> HotelRoom { get; set; } = new List<HotelRoomEntity>();
    }
}
