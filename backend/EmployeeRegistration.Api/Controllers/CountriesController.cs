using EmployeeRegistration.Api.Data;
using EmployeeRegistration.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRegistration.Api.Controllers;

[ApiController]
[Route("api/countries")]
public class CountriesController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountries()
    {
        return Ok(await db.Countries.AsNoTracking()
            .OrderBy(x => x.CountryName)
            .Select(x => new CountryDto { CountryId = x.CountryId, CountryName = x.CountryName })
            .ToListAsync());
    }

    [HttpGet("{countryId:int}/states")]
    public async Task<ActionResult<IEnumerable<StateDto>>> GetStatesByCountry(int countryId)
    {
        var exists = await db.Countries.AnyAsync(x => x.CountryId == countryId);
        if (!exists) return NotFound(new { message = "Country not found" });

        return Ok(await db.States.AsNoTracking()
            .Where(x => x.CountryId == countryId)
            .OrderBy(x => x.StateName)
            .Select(x => new StateDto
            {
                StateId = x.StateId,
                StateName = x.StateName,
                CountryId = x.CountryId
            })
            .ToListAsync());
    }
}
