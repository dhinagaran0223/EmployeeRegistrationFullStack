using EmployeeRegistration.Api.DTOs;
using EmployeeRegistration.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeRegistration.Api.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController(IEmployeeService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EmployeeListDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] string? name = null,
        [FromQuery] string? mobile = null)
    {
        return Ok(await service.GetPagedAsync(pageNumber, pageSize, name, mobile));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmployeeDetailDto>> GetById(int id)
    {
        var employee = await service.GetByIdAsync(id);
        return employee == null ? NotFound(new { message = "Employee not found" }) : Ok(employee);
    }

    [HttpPost]
    public async Task<ActionResult<EmployeeDetailDto>> Create(EmployeeCreateDto dto)
    {
        var (employee, error) = await service.CreateAsync(dto);
        if (error != null)
            return error.StartsWith("Already registered") ? Conflict(new { message = error }) : BadRequest(new { message = error });

        return CreatedAtAction(nameof(GetById), new { id = employee!.EmployeeId }, employee);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmployeeDetailDto>> Update(int id, EmployeeUpdateDto dto)
    {
        var (employee, error) = await service.UpdateAsync(id, dto);
        if (error != null)
        {
            if (error == "Employee not found") return NotFound(new { message = error });
            return error.StartsWith("Already registered") ? Conflict(new { message = error }) : BadRequest(new { message = error });
        }

        return Ok(employee);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound(new { message = "Employee not found" });
    }
}
