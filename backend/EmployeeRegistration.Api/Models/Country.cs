namespace EmployeeRegistration.Api.Models;

public class Country
{
    public int CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;

    public ICollection<State> States { get; set; } = new List<State>();
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
