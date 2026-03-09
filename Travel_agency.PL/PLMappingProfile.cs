using AutoMapper;
using Travel_agency.Core.BusinessModels.Hotels;
using Travel_agency.Core.BusinessModels.Tours;
using Travel_agency.Core.BusinessModels.Transports;
using Travel_agency.Core.BusinessModels.Users;
using Travel_agency.PL.Models.Requests;
using Travel_agency.PL.Models.Responses;

namespace Travel_agency.PL
{
    public class PLMappingProfile : Profile
    {
        public PLMappingProfile()
        {
            CreateMap<UserRequest, UserModel>();
            CreateMap<UserModel, UserResponse>();

            CreateMap<RegisterUserRequest, RegisterUserModel>();

            CreateMap<TransportRequest, TransportModel>();
            CreateMap<TransportModel, TransportResponse>();

            CreateMap<TourRequest, TourModel>();
            CreateMap<TourModel, TourResponse>();
            CreateMap<TourFilterRequest, TourFilterModel>();

            CreateMap<TourBookingRequest, TourBookingModel>();
            CreateMap<TourBookingModel, TourBookingResponse>();
            CreateMap<TourBookingDetailsModel, TourBookingDetailsResponse>();

            CreateMap<TicketBookingRequest, TicketBookingModel>();
            CreateMap<TicketBookingModel, TicketBookingResponse>();

            CreateMap<HotelRoomRequest, HotelRoomModel>();
            CreateMap<HotelRoomModel, HotelRoomResponse>();

            CreateMap<HotelRequest, HotelModel>();
            CreateMap<HotelModel, HotelResponse>();

            CreateMap<HotelBookingRequest, HotelBookingModel>();
            CreateMap<HotelBookingModel, HotelBookingResponse>();
            CreateMap<HotelBookingDetailsModel, HotelBookingDetailsResponse>();

        }
    }
}
