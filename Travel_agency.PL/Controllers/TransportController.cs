using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Travel_agency.BLL.Abstractions;
using Travel_agency.Core.BusinessModels.Transports;
using Travel_agency.PL.Models.Requests;
using Travel_agency.PL.Models.Responses;

namespace Travel_agency.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportController : ControllerBase
    {
        private readonly ITransportService _transportService;
        private readonly IMapper _mapper;

        public TransportController(ITransportService transportService, IMapper mapper)
        {
            _transportService = transportService;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllTransports()
        {
            var transports = await _transportService.GetAllTransportsAsync();
            return Ok(_mapper.Map<List<TransportResponse>>(transports));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTransportById(Guid id)
        {
            var transport = await _transportService.GetTransportByIdAsync(id);
            if (transport == null)
            {
                return NotFound();
            }
            return Ok(transport);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> CreateTransport([FromBody] TransportRequest transport)
        {
            if (transport == null)
            {
                return BadRequest("Transport data is null.");
            }

            var transportModel = _mapper.Map<TransportModel>(transport);

            var createdTransport = await _transportService.AddTransportAsync(transportModel);
            return Ok(_mapper.Map<TransportResponse>(createdTransport));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> UpdateTransport(Guid id, [FromBody] TransportRequest transport)
        {
            if (transport == null)
            {
                return BadRequest("Transport data is null.");
            }

            var transportModel = _mapper.Map<TransportModel>(transport);
            transportModel.Id = id;

            var updatedTransport = await _transportService.UpdateTransportAsync(transportModel);
            if (updatedTransport == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<TransportResponse>(updatedTransport));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,Manager")]
        public async Task<IActionResult> DeleteTransport(Guid id)
        {
            var transport = await _transportService.GetTransportByIdAsync(id);
            if (transport == null)
            {
                return NotFound();
            }

            await _transportService.DeleteTransportAsync(id);
            return NoContent();
        }
    }
}
