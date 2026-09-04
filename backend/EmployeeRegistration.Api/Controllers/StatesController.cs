using EmployeeRegistration.Api.Data;
using EmployeeRegistration.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRegistration.Api.Controllers;

[ApiController]
[Route("api/states")]
public class StatesController(AppDbContext db) : ControllerBase
{
    [HttpGet("{stateId:int}/country")]
    public async Task<ActionResult<CountryDto>> GetCountryByState(int stateId)
    {
        var country = await db.States.AsNoTracking()
            .Where(x => x.StateId == stateId)
            .Select(x => new CountryDto
            {
                CountryId = x.Country.CountryId,
                CountryName = x.Country.CountryName
            })
            .FirstOrDefaultAsync();

        return country == null ? NotFound(new { message = "State not found" }) : Ok(country);
    }
}
