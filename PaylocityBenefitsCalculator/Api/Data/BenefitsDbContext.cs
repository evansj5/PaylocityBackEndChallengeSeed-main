using Api.Controllers;
using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class BenefitsDbContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Dependent> Dependents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("MyDatabase");
        }
    }

    public class DataSeeder
    {
        public static void Seed(BenefitsDbContext context)
        {
            Employee[] employees = {
                new Employee 
                { 
                    Id = 1, 
                    FirstName = "LeBron", 
                    LastName = "James", 
                    Salary = 75420.99m, 
                    DateOfBirth = new DateTime(1984, 12, 30)
                },
                new Employee 
                { 
                    Id = 2, 
                    FirstName = "Ja", 
                    LastName = "Morant", 
                    Salary = 92365.22m, 
                    DateOfBirth = new DateTime(1999, 8, 10) ,
                    Dependents = new List<Dependent>
                    {
                        new Dependent { Id = 1, FirstName = "Spouse", LastName = "Morant", EmployeeId = 2, DateOfBirth = new DateTime(1998, 3, 3), Relationship = Relationship.Spouse },
                        new Dependent { Id = 2, FirstName = "Child1", LastName = "Morant", EmployeeId = 2, DateOfBirth = new DateTime(2020, 6, 23), Relationship = Relationship.Child },
                        new Dependent { Id = 3, FirstName = "Child2", LastName = "Morant", EmployeeId = 2, DateOfBirth = new DateTime(2021, 5, 18), Relationship = Relationship.Child }
                    }
                },
                new Employee 
                { 
                    Id = 3, 
                    FirstName = "Michael", 
                    LastName = "Jordan", 
                    Salary = 143211.12m, 
                    DateOfBirth = new DateTime(1963, 2, 17),
                    Dependents = new List<Dependent>
                    {
                        new Dependent { Id = 4, FirstName = "DP", LastName = "Jordan", EmployeeId = 3, DateOfBirth = new DateTime(1974, 1, 2), Relationship = Relationship.DomesticPartner }
                    }
                }
            };

            var dependents = employees
                .SelectMany(e =>
                {
                    foreach (var d in e.Dependents)
                    {
                        d.Employee = e;
                    }
                    return e.Dependents;
                })
                .ToArray();

            context.Employees.AddRange(employees);
            context.Dependents.AddRange(dependents);

            context.SaveChanges();
        }
    }
}