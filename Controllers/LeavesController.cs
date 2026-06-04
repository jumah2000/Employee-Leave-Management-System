using EmployeeLeaveManagementSystem.DTOs;
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

    [HttpGet]
    public async Task<IActionResult> GetAllLeaves()
    {
        var leaves = await _leaveRepository.GetAllLeaves();

        return Ok(leaves);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLeaveById(int id)
    {
        var leave = await _leaveRepository.GetLeaveById(id);

        return Ok(leave);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLeave(
        CreateLeaveDto createLeaveDto)
    {
        var leave =
            await _leaveRepository.CreateLeave(createLeaveDto);

        return Ok(leave);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLeave(
        int id,
        LeaveRequest leaveRequest)
    {
        if (id != leaveRequest.Id)
        {
            return BadRequest("Id mismatch");
        }

        var leave =
            await _leaveRepository.UpdateLeave(leaveRequest);

        return Ok(leave);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLeave(int id)
    {
        var result =
            await _leaveRepository.DeleteLeave(id);

        return Ok(result);
    }

    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetLeavesByStatus(
        string status)
    {
        var leaves =
            await _leaveRepository.GetLeavesByStatus(status);

        return Ok(leaves);
    }

    [HttpPut("{id}/approve")]
    public async Task<IActionResult> ApproveLeave(int id)
    {
        var leave =
            await _leaveRepository.ApproveLeave(id);

        return Ok(leave);
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> RejectLeave(int id)
    {
        var leave =
            await _leaveRepository.RejectLeave(id);

        return Ok(leave);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentLeaves()
    {
        var leaves =
            await _leaveRepository.GetCurrentLeaves();

        return Ok(leaves);
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetDepartmentStatistics()
    {
        var statistics =
            await _leaveRepository.GetDepartmentStatistics();

        return Ok(statistics);
    }
}
