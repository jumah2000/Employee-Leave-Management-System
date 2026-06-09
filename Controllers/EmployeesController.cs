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
    
    // GET ALL EMPLOYEES
    [HttpGet("GetAllEmployees")]
    public async Task<IActionResult> GetAllEmployees()
    {
        var employees = await _employeeRepository.GetAllEmployees();
        return Ok(employees);
    }
    
    // GET EMPLOYEE BY ID
    [HttpGet(" GetEmployeeById/{id}")]
    public async Task<IActionResult> GetEmployeeById(int id)
    {
        var employee = await _employeeRepository.GetEmployeeById(id);

        if (employee == null)
            return NotFound("Employee not found");

        return Ok(employee);
    }

    // CREATE EMPLOYEE
    [HttpPost("CreateEmployee")]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdEmployee = await _employeeRepository.CreateEmployee(dto);

        return CreatedAtAction(
            nameof(GetEmployeeById),
            new { id = createdEmployee.Id },
            createdEmployee
        );
    }
    
    // UPDATE EMPLOYEE
    [HttpPut("UpdateEmployee/{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedEmployee = await _employeeRepository.UpdateEmployee(id, dto);
            return Ok(updatedEmployee);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    // DELETE EMPLOYEE
    [HttpDelete("DeleteEmployee/{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        try
        {
            var result = await _employeeRepository.DeleteEmployee(id);

            if (!result)
                return BadRequest("Employee could not be deleted");

            return Ok("Employee deleted successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // GET EMPLOYEE LEAVE HISTORY
    [HttpGet("GetEmployeeLeaves/{id}")]
    public async Task<IActionResult> GetEmployeeLeaves(int id)
    {
        try
        {
            var leaves = await _employeeRepository.GetEmployeeLeaves(id);
            return Ok(leaves);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    // GET EMPLOYEES CURRENTLY ON LEAVE
    [HttpGet("GetEmployeesOnLeave")]
    public async Task<IActionResult> GetEmployeesOnLeave()
    {
        var employees = await _employeeRepository.GetEmployeesOnLeave();
        return Ok(employees);
    }

}
