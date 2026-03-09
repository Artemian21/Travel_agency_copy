using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Tours;
using Travel_agency.PL.Models.Requests;
using Travel_agency.PL.Models.Responses;

namespace Travel_agency.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;
        private readonly IMapper _mapper;

        public TourController(ITourService tourService, IMapper mapper)
        {
            _tourService = tourService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTours()
        {
            var tours = await _tourService.GetAllToursAsync();
            return Ok(_mapper.Map<List<TourResponse>>(tours));
        }

        [HttpGet("paged")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPagedTours([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedResult = await _tourService.GetPagedToursAsync(pageNumber, pageSize);
            return Ok(pagedResult);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTourById(Guid id)
        {
            var tour = await _tourService.GetTourByIdAsync(id);
            if (tour == null)
            {
                return NotFound();
            }
            return Ok(tour);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> CreateTour([FromBody] TourRequest tour)
        {
            if (tour == null)
            {
                return BadRequest("Tour data is null.");
            }

            var tourModel = _mapper.Map<TourModel>(tour);

            var createdTour = await _tourService.AddTourAsync(tourModel);
            return Ok(_mapper.Map<TourResponse>(createdTour));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> UpdateTour(Guid id, [FromBody] TourRequest tour)
        {
            if (tour == null)
            {
                return BadRequest("Tour data is null.");
            }

            var tourModel = _mapper.Map<TourModel>(tour);
            tourModel.Id = id;

            var updatedTour = await _tourService.UpdateTourAsync(tourModel);
            if (updatedTour == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TourResponse>(updatedTour));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Manager,Administrator")]
        public async Task<IActionResult> DeleteTour(Guid id)
        {
            var deleted = await _tourService.DeleteTourAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
