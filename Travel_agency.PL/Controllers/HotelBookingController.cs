using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Hotels;
using Travel_agency.PL.Models.Requests;
using Travel_agency.PL.Models.Responses;

namespace Travel_agency.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HotelBookingController : ControllerBase
    {
        private readonly IHotelBookingService _hotelBookingService;
        private readonly IMapper _mapper;

        public HotelBookingController(IHotelBookingService hotelBookingService, IMapper mapper)
        {
            _hotelBookingService = hotelBookingService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var allBookings = await _hotelBookingService.GetAllHotelBookingsAsync();

            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserRole) || string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User role or ID not found in claims.");
            }

            if (currentUserRole == "Administrator" || currentUserRole == "Manager")
            {
                return Ok(_mapper.Map<List<HotelBookingResponse>>(allBookings));
            }

            var userId = Guid.Parse(currentUserId);
            var userBookings = allBookings.Where(b => b.UserId == userId).ToList();

            return Ok(_mapper.Map<List<HotelBookingResponse>>(userBookings));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotelBookingById(Guid id)
        {
            var booking = await _hotelBookingService.GetHotelBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<HotelBookingDetailsResponse>(booking));
        }

        [HttpPost]
        public async Task<IActionResult> CreateHotelBooking([FromBody] HotelBookingRequest hotelBooking)
        {
            if (hotelBooking == null)
            {
                return BadRequest("Hotel booking data is null.");
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var hotelBookingModel = _mapper.Map<HotelBookingModel>(hotelBooking);
            hotelBookingModel.UserId = userId;

            var createdBooking = await _hotelBookingService.AddHotelBookingAsync(hotelBookingModel);
            return Ok(_mapper.Map<HotelBookingResponse>(createdBooking));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateHotelBooking(Guid id, [FromBody] HotelBookingRequest hotelBooking)
        {
            if (hotelBooking == null)
            {
                return BadRequest("Hotel booking data is null.");
            }

            var existingBooking = await _hotelBookingService.GetHotelBookingByIdAsync(id);
            if (existingBooking == null)
                return NotFound();

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var hotelBookingModel = _mapper.Map<HotelBookingModel>(hotelBooking);
            hotelBookingModel.Id = id;
            hotelBookingModel.UserId = existingBooking.UserId;

            var updatedBooking = await _hotelBookingService.UpdateHotelBookingAsync(hotelBookingModel);
            if (updatedBooking == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<HotelBookingResponse>(updatedBooking));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotelBooking(Guid id)
        {
            await _hotelBookingService.DeleteHotelBookingAsync(id);
            return NoContent();
        }
    }
}
