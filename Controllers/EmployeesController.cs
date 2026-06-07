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

    [HttpGet("GetAllEmployees")]
    public async Task<IActionResult> GetAllEmployees()
    {
        var employees = await _employeeRepository.GetAllEmployees();

        return Ok(employees);
    }

    [HttpGet("GetEmployeeById/{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var employee = await _employeeRepository.GetEmployeeById(id);

        return Ok(employee);
    }

    [HttpPost("CreateEmployee")]
    public async Task<IActionResult> CreateEmployee(CreateEmployeeDto createEmployeeDto)
    {
        var employee = await _employeeRepository.CreateEmployee(createEmployeeDto);

        return Ok(employee);
    }

    [HttpPut("UpdateEmployee/{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, CreateEmployeeDto dto)
    {
        if (dto == null)
            return BadRequest("Invalid data");

        var updatedEmployee = await _employeeRepository.UpdateEmployee(id, dto);

        return Ok(updatedEmployee);
    }
    [HttpDelete("DeleteEmployee/{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var result = await _employeeRepository.DeleteEmployee(id);

        return Ok(result);
    }

    [HttpGet("GetEmployeeLeaves/{id}")]
    public async Task<IActionResult> GetEmployeeLeaves(int id)
    {
        var leaves = await _employeeRepository.GetEmployeeLeaves(id);

        return Ok(leaves);
    }
}
