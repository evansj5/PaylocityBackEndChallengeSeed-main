using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Newtonsoft.Json;
using Xunit;

namespace ApiTests.IntegrationTests;

public class EmployeeIntegrationTests : IntegrationTest
{
    [Fact]
    public async Task WhenAskedForAllEmployees_ShouldReturnAllEmployees()
    {
        var response = await HttpClient.GetAsync("/api/v1/employees");
        var employees = new List<GetEmployeeDto>
        {
            new()
            {
                Id = 1,
                FirstName = "LeBron",
                LastName = "James",
                Salary = 75420.99m,
                DateOfBirth = new DateTime(1984, 12, 30)
            },
            new()
            {
                Id = 2,
                FirstName = "Ja",
                LastName = "Morant",
                Salary = 92365.22m,
                DateOfBirth = new DateTime(1999, 8, 10),
                Dependents = new List<GetDependentDto>
                {
                    new()
                    {
                        Id = 1,
                        FirstName = "Spouse",
                        LastName = "Morant",
                        Relationship = Relationship.Spouse,
                        DateOfBirth = new DateTime(1998, 3, 3)
                    },
                    new()
                    {
                        Id = 2,
                        FirstName = "Child1",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2020, 6, 23)
                    },
                    new()
                    {
                        Id = 3,
                        FirstName = "Child2",
                        LastName = "Morant",
                        Relationship = Relationship.Child,
                        DateOfBirth = new DateTime(2021, 5, 18)
                    }
                }
            },
            new()
            {
                Id = 3,
                FirstName = "Michael",
                LastName = "Jordan",
                Salary = 143211.12m,
                DateOfBirth = new DateTime(1963, 2, 17),
                Dependents = new List<GetDependentDto>
                {
                    new()
                    {
                        Id = 4,
                        FirstName = "DP",
                        LastName = "Jordan",
                        Relationship = Relationship.DomesticPartner,
                        DateOfBirth = new DateTime(1974, 1, 2)
                    }
                }
            },
                new () 
                { 
                    Id = 4, 
                    FirstName = "Larry", 
                    LastName = "Bird", 
                    Salary = 1413211.12m, 
                    DateOfBirth = new DateTime(1956, 12, 7),
                    Dependents = new List<GetDependentDto>
                    {
                        new () 
                        { 
                            Id = 5, 
                            FirstName = "Spouse", 
                            LastName = "Bird", 
                            DateOfBirth = new DateTime(1956, 1, 2), 
                            Relationship = Relationship.Spouse 
                        }
                    }
                }
        };
        await response.ShouldReturn(HttpStatusCode.OK, employees);
    }

    [Fact]
    //task: make test pass - check
    public async Task WhenAskedForAnEmployee_ShouldReturnCorrectEmployee()
    {
        var response = await HttpClient.GetAsync("/api/v1/employees/1");
        var employee = new GetEmployeeDto
        {
            Id = 1,
            FirstName = "LeBron",
            LastName = "James",
            Salary = 75420.99m,
            DateOfBirth = new DateTime(1984, 12, 30)
        };
        await response.ShouldReturn(HttpStatusCode.OK, employee);
    }

    [Fact]
    public async Task WhenAskedForAnEmployeePaycheck_ShouldCalculateCorrectly()
    {
        var response = await HttpClient.GetAsync("/api/v1/employees/2/paycheck");
        var paycheck = new GetPaycheckDto
        {
            GrossPay = 3552.5m,
            BaseBenefitsCosts = 461.53m,
            DependentsBenefitsCosts = 830.76m,
            SalaryBenefitsCosts = 71.05m,
            AgeBasedBenefitsCosts = 0m,
            NetPay = 2189.16m
        };
        await response.ShouldReturn(HttpStatusCode.OK, paycheck);
    }

    [Fact]
    public async Task WhenAskedForAnEmployeePaycheckWithDependentOver50_ShouldCalculateCorrectly() 
    {
        var response = await HttpClient.GetAsync("/api/v1/employees/4/paycheck");
        var paycheck = new GetPaycheckDto
        {
            GrossPay = 54354.27m,
            BaseBenefitsCosts = 461.53m,
            DependentsBenefitsCosts = 369.23m,
            SalaryBenefitsCosts = 1087.08m,
            AgeBasedBenefitsCosts = 92.3m,
            NetPay = 52344.13m
        };
        await response.ShouldReturn(HttpStatusCode.OK, paycheck);
    }

    [Fact]
    public async Task WhenAskedForAnEmployeePaycheckOver80000Salary_ShouldCalculateCorrectly()
    {
        var response = await HttpClient.GetAsync("/api/v1/employees/3/paycheck");
        var paycheck = new GetPaycheckDto
        {
            GrossPay = 5508.12m,
            BaseBenefitsCosts = 461.53m,
            DependentsBenefitsCosts = 276.92m,
            SalaryBenefitsCosts = 110.16m,
            AgeBasedBenefitsCosts = 92.3m,
            NetPay = 4567.21m
        };
        await response.ShouldReturn(HttpStatusCode.OK, paycheck);
    }
    
    [Fact]
    //task: make test pass - check
    public async Task WhenAskedForANonexistentEmployee_ShouldReturn404()
    {
        var response = await HttpClient.GetAsync($"/api/v1/employees/{int.MinValue}");
        await response.ShouldReturn(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenCreatingAnEmployeeWithValidData_ShouldReturnCreatedEmployee()
    {
        var employee = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Salary = 100000,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var jsonContent = JsonConvert.SerializeObject(employee);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/employees", content);
        var responseString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<ApiResponse<GetEmployeeDto>>(responseString);

        var createdEmployee = new GetEmployeeDto
        {
            Id = responseObject.Data.Id,
            FirstName = "Test",
            LastName = "Employee",
            Salary = 100000,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        await response.ShouldReturn(HttpStatusCode.Created, createdEmployee);
    }

    [Fact]
    public async Task WhenCreatingAnEmployeeWithTooManySpousesAndPartners_ShouldReturn400()
    {
        var employee = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Salary = 100000,
            DateOfBirth = new DateTime(1990, 1, 1),
            Dependents = new List<CreateDependentDto>()
            {
                new ()
                {
                    FirstName = "Spouse",
                    LastName = "1",
                    Relationship = Relationship.Spouse,
                    DateOfBirth = new DateTime(1990, 1, 1)
                },
                new ()
                {
                    FirstName = "DomesticPartner",
                    LastName = "2",
                    Relationship = Relationship.DomesticPartner,
                    DateOfBirth = new DateTime(1990, 1, 1)
                },
            }
        };

        var jsonContent = JsonConvert.SerializeObject(employee);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/employees", content);

        await response.ShouldReturn(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenCreatingAnEmployeeWithoutAFirstName_ShouldReturn400()
    {
        var employee = new CreateEmployeeDto
        {
            LastName = "Employee",
            Salary = 100000,
            DateOfBirth = new DateTime(1990, 1, 1),
        };

        var jsonContent = JsonConvert.SerializeObject(employee);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/employees", content);

        await response.ShouldReturn(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenCreatingAnEmployeeWithoutALastName_ShouldReturn400()
    {
        var employee = new CreateEmployeeDto
        {
            FirstName = "Test",
            Salary = 100000,
            DateOfBirth = new DateTime(1990, 1, 1),
        };

        var jsonContent = JsonConvert.SerializeObject(employee);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/employees", content);

        await response.ShouldReturn(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenCreatingAnEmployeeWithInvalidSalary_ShouldReturn400()
    {
        var employee = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Salary = -50,
            DateOfBirth = new DateTime(1990, 1, 1),
            Dependents = new List<CreateDependentDto>()
            {
                new ()
                {
                    FirstName = "Spouse",
                    LastName = "1",
                    Relationship = Relationship.Spouse,
                    DateOfBirth = new DateTime(1990, 1, 1)
                },
                new ()
                {
                    FirstName = "DomesticPartner",
                    LastName = "2",
                    Relationship = Relationship.DomesticPartner,
                    DateOfBirth = new DateTime(1990, 1, 1)
                },
            }
        };

        var jsonContent = JsonConvert.SerializeObject(employee);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/employees", content);

        await response.ShouldReturn(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenCreatingAnEmployeeWithNoDoB_ShouldReturn400()
    {
        var employee = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Salary = 10000,
        };

        var jsonContent = JsonConvert.SerializeObject(employee);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/employees", content);

        await response.ShouldReturn(HttpStatusCode.BadRequest);
    }
}

