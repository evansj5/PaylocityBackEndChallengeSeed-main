using System.Net;
using Api.Dtos.Dependent;
using Api.Dtos.Employee;
using Api.Models;
using Api.Services;
using Api.Validation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{    
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Get(int id)
    {
        var employee = _employeeService.Get(id);

        if (employee == null) 
        {
            return StatusCode((int)HttpStatusCode.NotFound, new ApiResponse<GetEmployeeDto>
            {
                Success = false,
                Error = $"Employee with id {id} not found"
            });
        }

        var result = new ApiResponse<GetEmployeeDto>
        {
            Data = employee,
            Success = true
        };

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<GetEmployeeDto>>>> GetAll()
    {
        var employees = _employeeService.GetAll();
        var result = new ApiResponse<List<GetEmployeeDto>>
        {
            Data = employees,
            Success = true
        };

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    [SwaggerOperation(Summary = "Get paycheck for employee")]
    [HttpGet("{employeeId}/paycheck")]
    public async Task<ActionResult<ApiResponse<GetPaycheckDto>>> GetPaycheck(int employeeId)
    {
        var paycheck = _employeeService.GetPaycheck(employeeId);

        if (paycheck == null) 
        {
            return StatusCode((int)HttpStatusCode.NotFound, new ApiResponse<GetPaycheckDto>
            {
                Success = false,
                Error = $"Employee with id {employeeId} not found"
            });
        }

        var result = new ApiResponse<GetPaycheckDto>
        {
            Data = paycheck,
            Success = true
        };

        return StatusCode((int)HttpStatusCode.OK, result);
    }

    [SwaggerOperation(Summary = "Create employee")]
    [HttpPost("")]
    public async Task<ActionResult<ApiResponse<GetEmployeeDto>>> Create(CreateEmployeeDto employeeRequest)
    {
        GetEmployeeDto employeeResponse;
        try 
        {
            employeeResponse = _employeeService.Create(employeeRequest);
        }
        catch (ValidationException e)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, new ApiResponse<GetEmployeeDto>
            {
                Success = false,
                Error = e.Message
            });
        }

        var result = new ApiResponse<GetEmployeeDto>
        {
            Data = employeeResponse,
            Success = true
        };

        return StatusCode((int)HttpStatusCode.Created, result);
    }
}
