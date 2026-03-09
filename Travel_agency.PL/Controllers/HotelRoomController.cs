using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Hotels;
using Travel_agency.PL.Models.Requests;
using Travel_agency.PL.Models.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Travel_agency.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelRoomController : ControllerBase
    {
        private readonly IHotelRoomService _hotelRoomService;
        private readonly IMapper _mapper;

        public HotelRoomController(IHotelRoomService hotelRoomService, IMapper mapper)
        {
            _hotelRoomService = hotelRoomService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllHotelRooms()
        {
            var hotelRooms = await _hotelRoomService.GetAllHotelRoomsAsync();
            return Ok(_mapper.Map<List<HotelRoomResponse>>(hotelRooms));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelRoomById(Guid id)
        {
            var hotelRoom = await _hotelRoomService.GetHotelRoomByIdAsync(id);
            if (hotelRoom == null)
            {
                return NotFound();
            }
            return Ok(hotelRoom);
        }

        [HttpGet("hotel/{hotelId}/rooms")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelRoomsByHotelId(Guid hotelId)
        {
            var rooms = await _hotelRoomService.GetHotelRoomsByHotelIdAsync(hotelId);
            return Ok(_mapper.Map<List<HotelRoomResponse>>(rooms));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> CreateHotelRoom([FromBody] HotelRoomRequest hotelRoom)
        {
            if (hotelRoom == null)
            {
                return BadRequest("Invalid hotel room data");
            }

            var hotelRoomModel = _mapper.Map<HotelRoomModel>(hotelRoom);

            var createdHotelRoom = await _hotelRoomService.AddHotelRoomAsync(hotelRoomModel);
            return Ok(_mapper.Map<HotelRoomResponse>(createdHotelRoom));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> UpdateHotelRoom(Guid id, [FromBody] HotelRoomRequest hotelRoom)
        {
            if (hotelRoom == null)
            {
                return BadRequest("Invalid hotel room data");
            }

            var hotelRoomModel = _mapper.Map<HotelRoomModel>(hotelRoom);
            hotelRoomModel.Id = id;

            var updatedHotelRoom = await _hotelRoomService.UpdateHotelRoomAsync(hotelRoomModel);
            if (updatedHotelRoom == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<HotelRoomResponse>(updatedHotelRoom));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> DeleteHotelRoom(Guid id)
        {
            var hotelRoom = await _hotelRoomService.GetHotelRoomByIdAsync(id);
            if (hotelRoom == null)
            {
                return NotFound();
            }

            await _hotelRoomService.DeleteHotelRoomAsync(id);
            return NoContent();
        }
    }
}
