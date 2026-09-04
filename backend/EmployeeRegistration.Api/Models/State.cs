namespace EmployeeRegistration.Api.Models;

public class State
{
    public int StateId { get; set; }
    public string StateName { get; set; } = string.Empty;

    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
