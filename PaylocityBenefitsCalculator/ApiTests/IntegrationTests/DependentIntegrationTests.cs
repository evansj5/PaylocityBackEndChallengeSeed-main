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

public class DependentIntegrationTests : IntegrationTest
{
    [Fact]
    //task: make test pass - check
    public async Task WhenAskedForAllDependents_ShouldReturnAllDependents()
    {
        var response = await HttpClient.GetAsync("/api/v1/dependents");
        var dependents = new List<GetDependentDto>
        {
            new()
            {
                Id = 1,
                FirstName = "Spouse",
                LastName = "Morant",
                Relationship = Relationship.Spouse,
                DateOfBirth = new DateTime(1998, 3, 3),
            },
            new()
            {
                Id = 2,
                FirstName = "Child1",
                LastName = "Morant",
                Relationship = Relationship.Child,
                DateOfBirth = new DateTime(2020, 6, 23),
            },
            new()
            {
                Id = 3,
                FirstName = "Child2",
                LastName = "Morant",
                Relationship = Relationship.Child,
                DateOfBirth = new DateTime(2021, 5, 18),
            },
            new()
            {
                Id = 4,
                FirstName = "DP",
                LastName = "Jordan",
                Relationship = Relationship.DomesticPartner,
                DateOfBirth = new DateTime(1974, 1, 2),
            },
            new () 
            { 
                Id = 5, 
                FirstName = "Spouse", 
                LastName = "Bird", 
                DateOfBirth = new DateTime(1956, 1, 2), 
                Relationship = Relationship.Spouse,
            },
            new () 
            { 
                Id = 6, 
                FirstName = "Spouse", 
                LastName = "Durant", 
                DateOfBirth = new DateTime(1988, 1, 2), 
                Relationship = Relationship.Spouse,
            }
        };
        await response.ShouldReturn(HttpStatusCode.OK, dependents);
    }

    [Fact]
    //task: make test pass - check
    public async Task WhenAskedForADependent_ShouldReturnCorrectDependent()
    {
        var response = await HttpClient.GetAsync("/api/v1/dependents/1");
        var dependent = new GetDependentDto
        {
            Id = 1,
            FirstName = "Spouse",
            LastName = "Morant",
            Relationship = Relationship.Spouse,
            DateOfBirth = new DateTime(1998, 3, 3),
        };
        await response.ShouldReturn(HttpStatusCode.OK, dependent);
    }

    [Fact]
    //task: make test pass - check
    public async Task WhenAskedForANonexistentDependent_ShouldReturn404()
    {
        var response = await HttpClient.GetAsync($"/api/v1/dependents/{int.MinValue}");
        await response.ShouldReturn(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task WhenCreatingADependentWithValidData_ShouldReturnCreatedDependent()
    {
        var dependent = new CreateDependentDto
        {
            FirstName = "Test",
            LastName = "Dependent",
            DateOfBirth = new DateTime(1990, 1, 1),
            Relationship = Relationship.Child,
            EmployeeId = 5
        };

        var jsonContent = JsonConvert.SerializeObject(dependent);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/dependents", content);
        var responseString = await response.Content.ReadAsStringAsync();
        var responseObject = JsonConvert.DeserializeObject<ApiResponse<GetDependentDto>>(responseString);

        var createdDependent = new GetDependentDto
        {
            Id = responseObject.Data == null ? 0 : responseObject.Data.Id,
            FirstName = "Test",
            LastName = "Dependent",
            DateOfBirth = new DateTime(1990, 1, 1),
            Relationship = Relationship.Child
        };

        await response.ShouldReturn(HttpStatusCode.Created, createdDependent);
    }

    [Fact]
    public async Task WhenCreatingADependentWithInvalidFirstName_ShouldReturn400()
    {
        var dependent = new CreateDependentDto
        {
            FirstName = "",
            LastName = "Dependent",
            DateOfBirth = new DateTime(1990, 1, 1),
            Relationship = Relationship.Child,
            EmployeeId = 5
        };

        var jsonContent = JsonConvert.SerializeObject(dependent);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/dependents", content);

        await response.ShouldReturn(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenCreatingADependentWithInvalidLastName_ShouldReturn400()
    {
        var dependent = new CreateDependentDto
        {
            FirstName = "Test",
            LastName = "",
            DateOfBirth = new DateTime(1990, 1, 1),
            Relationship = Relationship.Child,
            EmployeeId = 5
        };

        var jsonContent = JsonConvert.SerializeObject(dependent);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/dependents", content);

        await response.ShouldReturn(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenCreatingADependentWithNoDateOfBirth_ShouldReturn400()
    {
        var dependent = new CreateDependentDto
        {
            FirstName = "Test",
            LastName = "Dependent",
            Relationship = Relationship.Child,
            EmployeeId = 5
        };

        var jsonContent = JsonConvert.SerializeObject(dependent);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/dependents", content);

        await response.ShouldReturn(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenCreatingADependentWithNoEmployeeId_ShouldReturn400()
    {
        var dependent = new CreateDependentDto
        {
            FirstName = "Test",
            LastName = "Dependent",
            Relationship = Relationship.Child,
            DateOfBirth = new DateTime(1990, 1, 1)
        };

        var jsonContent = JsonConvert.SerializeObject(dependent);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/dependents", content);

        await response.ShouldReturn(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task WhenCreatingASpouseDependentWithEmployeeThatAlreadyHasOne_ShouldReturn400()
    {
        var employee = new CreateEmployeeDto
        {
            FirstName = "Test",
            LastName = "Employee",
            Salary = 100000,
            DateOfBirth = new DateTime(1990, 1, 1),
            Dependents = new List<CreateDependentDto>
            {
                new ()
                {
                    FirstName = "Test",
                    LastName = "Dependent",
                    Relationship = Relationship.Spouse,
                    DateOfBirth = new DateTime(1990, 1, 1)
                }
            }
        };

        var employeeJsonContent = JsonConvert.SerializeObject(employee);
        var employeeContent = new StringContent(employeeJsonContent, System.Text.Encoding.UTF8, "application/json");
        var employeeResponse = await HttpClient.PostAsync("/api/v1/employees", employeeContent);
        var employeeResponseString = await employeeResponse.Content.ReadAsStringAsync();
        var employeeResponseObject = JsonConvert.DeserializeObject<ApiResponse<GetEmployeeDto>>(employeeResponseString);

        var dependent = new CreateDependentDto
        {
            FirstName = "Test2",
            LastName = "Dependent",
            Relationship = Relationship.Spouse,
            DateOfBirth = new DateTime(1990, 1, 1),
            EmployeeId = employeeResponseObject.Data.Id
        };

        var jsonContent = JsonConvert.SerializeObject(dependent);
        var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync("/api/v1/dependents", content);

        await response.ShouldReturn(HttpStatusCode.BadRequest);
    }
}

