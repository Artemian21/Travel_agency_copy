using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Transports;
using Travel_agency.PL.Models.Requests;
using Travel_agency.PL.Models.Responses;

namespace Travel_agency.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketBookingController : ControllerBase
    {
        private readonly ITicketBookingService _ticketBookingService;
        private readonly IMapper _mapper;

        public TicketBookingController(ITicketBookingService ticketBookingService, IMapper mapper)
        {
            _ticketBookingService = ticketBookingService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBookings()
        {
            var allBookings = await _ticketBookingService.GetAllTicketBookingsAsync();

            var currentUserRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var currentUserId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserRole) || string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("User role or ID not found in claims.");
            }

            if (currentUserRole == "Administrator" || currentUserRole == "Manager")
            {
                return Ok(_mapper.Map<List<TicketBookingResponse>>(allBookings));
            }

            var userId = Guid.Parse(currentUserId);
            var userBookings = allBookings.Where(b => b.UserId == userId).ToList();
            return Ok(_mapper.Map<List<TicketBookingResponse>>(userBookings));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketBookingById(Guid id)
        {
            var booking = await _ticketBookingService.GetTicketBookingByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicketBooking([FromBody] TicketBookingRequest ticketBooking)
        {
            if (ticketBooking == null)
            {
                return BadRequest("Ticket booking data is null.");
            }

            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var userId = Guid.Parse(userIdClaim.Value);

            var ticketBookingModel = new TicketBookingModel
            {
                UserId = userId,
                TransportId = ticketBooking.TransportId,
                Status = ticketBooking.Status
            };

            var createdBooking = await _ticketBookingService.AddTicketBookingAsync(ticketBookingModel);
            return CreatedAtAction(nameof(GetTicketBookingById), new { id = createdBooking.Id }, createdBooking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTicketBooking(Guid id, [FromBody] TicketBookingRequest ticketBooking)
        {
            if (ticketBooking == null)
            {
                return BadRequest("Ticket booking data is null.");
            }

            var existingBooking = await _ticketBookingService.GetTicketBookingByIdAsync(id);
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

            var ticketBookingModel = _mapper.Map<TicketBookingModel>(ticketBooking);
            ticketBookingModel.Id = id;
            ticketBookingModel.UserId = existingBooking.UserId;


            var updatedBooking = await _ticketBookingService.UpdateTicketBookingAsync(ticketBookingModel);
            if (updatedBooking == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TicketBookingResponse>(updatedBooking));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTicketBooking(Guid id)
        {
            var deleted = await _ticketBookingService.DeleteTicketBookingAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
