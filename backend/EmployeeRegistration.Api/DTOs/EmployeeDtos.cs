namespace EmployeeRegistration.Api.DTOs;

public class EmployeeCreateDto
{
    public string EmployeeName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string MobileNum { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public DateTime? DOB { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public int StateId { get; set; }
    public int CountryId { get; set; }
}

public class EmployeeUpdateDto : EmployeeCreateDto
{
}

public class EmployeeListDto
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string MobileNum { get; set; } = string.Empty;
}

public class EmployeeDetailDto : EmployeeListDto
{
    public string Pincode { get; set; } = string.Empty;
    public DateTime? DOB { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public int StateId { get; set; }
    public string StateName { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
}
