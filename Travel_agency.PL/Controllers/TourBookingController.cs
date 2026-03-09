using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Tours;
using Travel_agency.PL.Models.Requests;
using Travel_agency.PL.Models.Responses;

namespace Travel_agency.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TourBookingController : ControllerBase
    {
        private readonly ITourBookingService _tourBookingService;
        private readonly IMapper _mapper;

        public TourBookingController(ITourBookingService tourBookingService, IMapper mapper)
        {
            _tourBookingService = tourBookingService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var allBookings = await _tourBookingService.GetAllTourBookingsAsync();

            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserRole) || string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User role or ID not found in claims.");
            }

            if (currentUserRole == "Administrator" || currentUserRole == "Manager")
            {
                return Ok(_mapper.Map<List<TourBookingResponse>>(allBookings));
            }

            var userId = Guid.Parse(currentUserId);
            var userBookings = allBookings.Where(b => b.UserId == userId).ToList();
            return Ok(_mapper.Map<List<TourBookingResponse>>(userBookings));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTourBookingById(Guid id)
        {
            var booking = await _tourBookingService.GetTourBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TourBookingDetailsResponse>(booking));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTourBooking([FromBody] TourBookingRequest tourBooking)
        {
            if (tourBooking == null)
            {
                return BadRequest("Tour booking data is null.");
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var tourBookingModel = _mapper.Map<TourBookingModel>(tourBooking);
            tourBookingModel.UserId = userId;

            var createdBooking = await _tourBookingService.AddTourBookingAsync(tourBookingModel);
            return Ok(_mapper.Map<TourBookingResponse>(createdBooking));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTourBooking(Guid id, [FromBody] TourBookingRequest tourBooking)
        {
            if (tourBooking == null)
            {
                return BadRequest("Tour booking data is null.");
            }

            var existingBooking = await _tourBookingService.GetTourBookingByIdAsync(id);
            if (existingBooking == null)
            {
                return NotFound();
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var tourBookingModel = _mapper.Map<TourBookingModel>(tourBooking);
            tourBookingModel.Id = id;
            tourBookingModel.UserId = existingBooking.UserId;

            var updatedBooking = await _tourBookingService.UpdateTourBookingAsync(tourBookingModel);
            if (updatedBooking == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TourBookingResponse>(updatedBooking));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTourBooking(Guid id)
        {
            var deleted = await _tourBookingService.DeleteTourBookingAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
