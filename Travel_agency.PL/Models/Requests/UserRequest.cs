using System.ComponentModel.DataAnnotations;

namespace Travel_agency.PL.Models.Requests
{
    public record UserRequest(
        string Username,
        [EmailAddress] string Email
        );
}
