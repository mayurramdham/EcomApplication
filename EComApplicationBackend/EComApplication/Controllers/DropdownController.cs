using App.core.App.Dropdown;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EComApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DropdownController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DropdownController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("GetCountries")]
        public async Task<ActionResult> GetCountries()
        {
            var result = await _mediator.Send(new GetCountriesQuery());
            return Ok(result);
        }
        [HttpGet("GetStatesByCountry/{countryId}")]
        public async Task<ActionResult> GetStatesByCountry(int countryId)
        {
            var result = await _mediator.Send(new GetStatesByCountryQuery(countryId));
            return Ok(result);
        }
    }
}
