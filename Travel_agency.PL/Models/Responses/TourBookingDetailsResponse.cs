using Travel_agency.Core.BusinessModels.Tours;
using Travel_agency.Core.BusinessModels.Users;
using Travel_agency.Core.Enums;

namespace Travel_agency.PL.Models.Responses
{
    public record TourBookingDetailsResponse(
        Guid Id,
        Guid TourId,
        Guid UserId,
        DateTime BookingDate,
        Status Status,
        TourModel Tour,
        UserModel User);
}
