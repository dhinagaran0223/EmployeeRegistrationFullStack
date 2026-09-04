using System.Text.RegularExpressions;
using EmployeeRegistration.Api.Data;
using EmployeeRegistration.Api.DTOs;
using EmployeeRegistration.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRegistration.Api.Services;

public class EmployeeService(AppDbContext db) : IEmployeeService
{
    private static readonly Regex NameRegex = new(@"^[A-Za-z ]+$", RegexOptions.Compiled);
    private static readonly Regex MobileRegex = new(@"^\d{10}$", RegexOptions.Compiled);
    private static readonly Regex PincodeRegex = new(@"^\d{6}$", RegexOptions.Compiled);
    private static readonly Regex AgeRegex = new(@"^\d{1,3}$", RegexOptions.Compiled);
    private static readonly Regex AddressRegex = new(@"[$%!+]", RegexOptions.Compiled);

    public async Task<PagedResult<EmployeeListDto>> GetPagedAsync(
        int pageNumber, int pageSize, string? name, string? mobile)
    {
        pageNumber = Math.Max(1, pageNumber);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Employees.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(x => x.EmployeeName.Contains(name.Trim()));

        if (!string.IsNullOrWhiteSpace(mobile))
            query = query.Where(x => x.MobileNum.Contains(mobile.Trim()));

        var total = await query.CountAsync();

        var items = await query
            .OrderBy(x => x.EmployeeId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new EmployeeListDto
            {
                EmployeeId = x.EmployeeId,
                EmployeeName = x.EmployeeName,
                Age = x.Age,
                MobileNum = x.MobileNum
            })
            .ToListAsync();

        return new PagedResult<EmployeeListDto>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalRecords = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
    }

    public async Task<EmployeeDetailDto?> GetByIdAsync(int id)
    {
        return await db.Employees.AsNoTracking()
            .Where(x => x.EmployeeId == id)
            .Select(x => new EmployeeDetailDto
            {
                EmployeeId = x.EmployeeId,
                EmployeeName = x.EmployeeName,
                Age = x.Age,
                MobileNum = x.MobileNum,
                Pincode = x.Pincode,
                DOB = x.DOB,
                AddressLine1 = x.AddressLine1,
                AddressLine2 = x.AddressLine2,
                StateId = x.StateId,
                StateName = x.State.StateName,
                CountryId = x.CountryId,
                CountryName = x.Country.CountryName
            })
            .FirstOrDefaultAsync();
    }

    public async Task<(EmployeeDetailDto? Employee, string? Error)> CreateAsync(EmployeeCreateDto dto)
    {
        var error = await ValidateAsync(dto, null);
        if (error != null) return (null, error);

        var employee = new Employee();
        Apply(employee, dto);

        db.Employees.Add(employee);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Employee_Mst_MobileNum") == true)
        {
            return (null, "Already registered user. Please enter a new one");
        }

        return (await GetByIdAsync(employee.EmployeeId), null);
    }

    public async Task<(EmployeeDetailDto? Employee, string? Error)> UpdateAsync(int id, EmployeeUpdateDto dto)
    {
        var employee = await db.Employees.FindAsync(id);
        if (employee == null) return (null, "Employee not found");

        var error = await ValidateAsync(dto, id);
        if (error != null) return (null, error);

        Apply(employee, dto);
        try
        {
            await db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("IX_Employee_Mst_MobileNum") == true)
        {
            return (null, "Already registered user. Please enter a new one");
        }

        return (await GetByIdAsync(id), null);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await db.Employees.FindAsync(id);
        if (employee == null) return false;

        db.Employees.Remove(employee);
        await db.SaveChangesAsync();
        return true;
    }

    private async Task<string?> ValidateAsync(EmployeeCreateDto dto, int? employeeId)
    {
        if (string.IsNullOrWhiteSpace(dto.EmployeeName) || dto.EmployeeName.Length > 30 || !NameRegex.IsMatch(dto.EmployeeName.Trim()))
            return "Employee Name is mandatory and allows only alphabets and spaces (maximum 30 characters).";

        if (!AgeRegex.IsMatch(dto.Age.ToString()) || dto.Age < 1 || dto.Age > 999)
            return "Age must contain 1 to 3 digits.";

        if (!MobileRegex.IsMatch(dto.MobileNum ?? string.Empty))
            return "MobileNum must be exactly 10 digits.";

        var duplicate = await db.Employees.AnyAsync(x =>
            x.MobileNum == dto.MobileNum && (!employeeId.HasValue || x.EmployeeId != employeeId.Value));

        if (duplicate)
            return "Already registered user. Please enter a new one";

        if (string.IsNullOrWhiteSpace(dto.AddressLine1) || dto.AddressLine1.Length > 250 || AddressRegex.IsMatch(dto.AddressLine1))
            return "Address Line 1 is mandatory, maximum 250 characters, and cannot contain $, %, !, +.";

        if (!string.IsNullOrEmpty(dto.AddressLine2) && (dto.AddressLine2.Length > 250 || AddressRegex.IsMatch(dto.AddressLine2)))
            return "Address Line 2 cannot contain $, %, !, + and must be maximum 250 characters.";

        if (!PincodeRegex.IsMatch(dto.Pincode ?? string.Empty))
            return "Pincode must be exactly 6 digits.";

        if (dto.DOB.HasValue && dto.DOB.Value.Date > DateTime.Today)
            return "Date of Birth cannot be a future date.";

        if (dto.DOB.HasValue)
        {
            var calculatedAge = CalculateAge(dto.DOB.Value.Date);
            if (calculatedAge != dto.Age)
                return "Age does not match Date of Birth.";
        }

        var countryExists = await db.Countries.AnyAsync(x => x.CountryId == dto.CountryId);
        if (!countryExists) return "Selected Country is invalid.";

        var stateExists = await db.States.AnyAsync(x => x.StateId == dto.StateId && x.CountryId == dto.CountryId);
        if (!stateExists) return "Selected State does not belong to the selected Country.";

        return null;
    }

    private static void Apply(Employee employee, EmployeeCreateDto dto)
    {
        employee.EmployeeName = dto.EmployeeName.Trim();
        employee.Age = dto.Age;
        employee.MobileNum = dto.MobileNum;
        employee.Pincode = dto.Pincode;
        employee.DOB = dto.DOB?.Date;
        employee.AddressLine1 = dto.AddressLine1.Trim();
        employee.AddressLine2 = string.IsNullOrWhiteSpace(dto.AddressLine2) ? null : dto.AddressLine2.Trim();
        employee.StateId = dto.StateId;
        employee.CountryId = dto.CountryId;
    }

    public static int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }
}
