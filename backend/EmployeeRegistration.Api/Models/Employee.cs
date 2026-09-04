namespace EmployeeRegistration.Api.Models;

public class Employee
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int Age { get; set; }
    public string MobileNum { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;
    public DateTime? DOB { get; set; }
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }

    public int StateId { get; set; }
    public State State { get; set; } = null!;

    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;
}
