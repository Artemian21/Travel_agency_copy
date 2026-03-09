using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Hotels;
using Travel_agency.PL.Models.Requests;
using Travel_agency.PL.Models.Responses;


namespace Travel_agency.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        private readonly IMapper _mapper;

        public HotelController(IHotelService hotelService, IMapper mapper)
        {
            _hotelService = hotelService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllHotels()
        {
            var hotels = await _hotelService.GetAllHotelsAsync();
            return Ok(_mapper.Map<List<HotelResponse>>(hotels));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetHotelById(Guid id)
        {
            var hotel = await _hotelService.GetHotelByIdAsync(id);
            return Ok(hotel);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> CreateHotel([FromBody] HotelRequest hotel)
        {
            if (hotel == null)
            {
                return BadRequest("Invalid hotel data");
            }

            var hotelModel = _mapper.Map<HotelModel>(hotel);

            var createdHotel = await _hotelService.AddHotelAsync(hotelModel);
            return Ok(_mapper.Map<HotelResponse>(createdHotel));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] HotelRequest hotel)
        {
            if (hotel == null)
            {
                return BadRequest("Invalid hotel data");
            }

            var hotelModel = _mapper.Map<HotelModel>(hotel);
            hotelModel.Id = id;

            var updatedHotel = await _hotelService.UpdateHotelAsync(hotelModel);
            if (updatedHotel == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<HotelResponse>(updatedHotel));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> DeleteHotel(Guid id)
        {
            await _hotelService.DeleteHotelAsync(id);
            return NoContent();
        }
    }
}
