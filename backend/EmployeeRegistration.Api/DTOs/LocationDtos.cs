namespace EmployeeRegistration.Api.DTOs;

public class CountryDto
{
    public int CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;
}

public class StateDto
{
    public int StateId { get; set; }
    public string StateName { get; set; } = string.Empty;
    public int CountryId { get; set; }
}
