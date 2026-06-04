using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Model;
using EmployeeLeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeesController(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllEmployees()
    {
        var employees = await _employeeRepository.GetAllEmployees();

        return Ok(employees);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var employee = await _employeeRepository.GetEmployeeById(id);

        return Ok(employee);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEmployee(
        CreateEmployeeDto createEmployeeDto)
    {
        var employee =
            await _employeeRepository.CreateEmployee(createEmployeeDto);

        return Ok(employee);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(
        int id,
        Employee employee)
    {
        if (id != employee.Id)
        {
            return BadRequest("Id mismatch");
        }

        var updatedEmployee =
            await _employeeRepository.UpdateEmployee(employee);

        return Ok(updatedEmployee);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var result = await _employeeRepository.DeleteEmployee(id);

        return Ok(result);
    }

    [HttpGet("{id}/leaves")]
    public async Task<IActionResult> GetEmployeeLeaves(int id)
    {
        var leaves =
            await _employeeRepository.GetEmployeeLeaves(id);

        return Ok(leaves);
    }
}
