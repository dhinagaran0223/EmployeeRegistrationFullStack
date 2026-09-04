using EmployeeRegistration.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeRegistration.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Countries.AnyAsync())
        {
            db.Countries.AddRange(
                new Country { CountryName = "India" },
                new Country { CountryName = "United States" },
                new Country { CountryName = "United Kingdom" }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.States.AnyAsync())
        {
            var countries = await db.Countries.ToDictionaryAsync(x => x.CountryName, x => x.CountryId);

            db.States.AddRange(
                new State { StateName = "Tamil Nadu", CountryId = countries["India"] },
                new State { StateName = "Puducherry", CountryId = countries["India"] },
                new State { StateName = "Karnataka", CountryId = countries["India"] },
                new State { StateName = "California", CountryId = countries["United States"] },
                new State { StateName = "Texas", CountryId = countries["United States"] },
                new State { StateName = "New York", CountryId = countries["United States"] },
                new State { StateName = "England", CountryId = countries["United Kingdom"] },
                new State { StateName = "Scotland", CountryId = countries["United Kingdom"] }
            );
            await db.SaveChangesAsync();
        }
    }
}
