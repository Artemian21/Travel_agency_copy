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
    public class TourFilterController : ControllerBase
    {
        private readonly ITourQueryService _tourQueryService;
        private readonly IMapper _mapper;

        public TourFilterController(ITourQueryService tourQueryService, IMapper mapper)
        {
            _tourQueryService = tourQueryService;
            _mapper = mapper;
        }

        [HttpPost("filter")]
        [AllowAnonymous]
        public async Task<IActionResult> FilterTours([FromBody] TourFilterRequest tourFilter)
        {
            var filteredTours = await _tourQueryService.GetFilteredToursAsync(_mapper.Map<TourFilterModel>(tourFilter));
            return Ok(_mapper.Map<List<TourResponse>>(filteredTours));
        }

        [HttpGet("search/{searchQuary}")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchTours(string searchQuary)
        {
            var searchedTours = await _tourQueryService.SearchToursAsync(searchQuary);
            return Ok(_mapper.Map<List<TourResponse>>(searchedTours));
        }
    }
}
