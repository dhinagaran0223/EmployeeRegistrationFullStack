using EmployeeRegistration.Api.DTOs;

namespace EmployeeRegistration.Api.Services;

public interface IEmployeeService
{
    Task<EmployeeDetailDto?> GetByIdAsync(int id);
    Task<PagedResult<EmployeeListDto>> GetPagedAsync(int pageNumber, int pageSize, string? name, string? mobile);
    Task<(EmployeeDetailDto? Employee, string? Error)> CreateAsync(EmployeeCreateDto dto);
    Task<(EmployeeDetailDto? Employee, string? Error)> UpdateAsync(int id, EmployeeUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}
