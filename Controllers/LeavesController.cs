using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Enums;
using EmployeeLeaveManagementSystem.Model;
using EmployeeLeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeLeaveManagementSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeavesController: ControllerBase
{
    private readonly ILeaveRepository _leaveRepository;

    public LeavesController(ILeaveRepository leaveRepository)
    {
        _leaveRepository = leaveRepository;
    }
    
    // GET ALL LEAVES
    [HttpGet]
    public async Task<IActionResult> GetAllLeaves()
    {
        var leaves = await _leaveRepository.GetAllLeaves();
        return Ok(leaves);
    }
    
    // GET LEAVE BY ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetLeaveById(int id)
    {
        try
        {
            var leave = await _leaveRepository.GetLeaveById(id);
            return Ok(leave);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    // SUBMIT LEAVE (CREATE)
    [HttpPost]
    public async Task<IActionResult> CreateLeave([FromBody] SubmitLeaveRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var leave = await _leaveRepository.CreateLeave(dto);

            return CreatedAtAction(
                nameof(GetLeaveById),
                new { id = leave.Id },
                leave
            );
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // UPDATE LEAVE REQUEST
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLeave(int id, [FromBody] SubmitLeaveRequestDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _leaveRepository.UpdateLeave(id, dto);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // DELETE LEAVE
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLeave(int id)
    {
        try
        {
            var result = await _leaveRepository.DeleteLeave(id);

            if (!result)
                return BadRequest("Could not delete leave");

            return Ok("Leave deleted successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // APPROVE LEAVE (2-STEP WORKFLOW)
    [HttpPost("{id}/approve")]
    public async Task<IActionResult> ApproveLeave(int id, [FromBody] LeaveActionRequestDto dto)
    {
        try
        {
            var result = await _leaveRepository.ApproveLeave(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // REJECT LEAVE (FINAL STATE)
    [HttpPost("{id}/reject")]
    public async Task<IActionResult> RejectLeave(int id, [FromBody] LeaveActionRequestDto dto)
    {
        try
        {
            var result = await _leaveRepository.RejectLeave(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    // GET LEAVES BY STATUS
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetByStatus(LeaveStatus status)
    {
        var leaves = await _leaveRepository.GetLeavesByStatus(status);
        return Ok(leaves);
    }
    
    // EMPLOYEE LEAVE HISTOR
    [HttpGet("employee/{employeeId}/history")]
    public async Task<IActionResult> GetEmployeeLeaveHistory(int employeeId)
    {
        try
        {
            var history = await _leaveRepository.GetEmployeeLeaveHistory(employeeId);
            return Ok(history);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    // EMPLOYEES CURRENTLY ON LEAVE
    [HttpGet("employees/on-leave")]
    public async Task<IActionResult> GetEmployeesOnLeave()
    {
        var result = await _leaveRepository.GetEmployeesOnLeave();
        return Ok(result);
    }
    
    // LEAVE STATISTICS (BY DEPARTMENT)
    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics()
    {
        var stats = await _leaveRepository.GetLeaveStatisticsByDepartment();
        return Ok(stats);
    }

}