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

    [HttpGet("GetAllLeaves")]
    public async Task<IActionResult> GetAllLeaves()
    {
        var leaves = await _leaveRepository.GetAllLeaves();

        return Ok(leaves);
    }

    [HttpGet("GetLeaveById/{id}")]
    public async Task<IActionResult> GetLeaveById(int id)
    {
        var leave = await _leaveRepository.GetLeaveById(id);

        return Ok(leave);
    }

    [HttpPost("CreateLeave")]
    public async Task<IActionResult> CreateLeave(CreateLeaveDto createLeaveDto)
    {
        var leave = await _leaveRepository.CreateLeave(createLeaveDto);

        return Ok(leave);
    }

    [HttpPut("UpdateLeave/{id}")]
    public async Task<IActionResult> UpdateLeave(int id, [FromBody]CreateLeaveDto dto)
    {
        var updatedLeave = await _leaveRepository.UpdateLeave(id, dto);

        return Ok(updatedLeave);
    }
    [HttpDelete("DeleteLeave/{id}")]
    public async Task<IActionResult> DeleteLeave(int id)
    {
        var result = await _leaveRepository.DeleteLeave(id);

        return Ok(result);
    }

    [HttpGet("Status/{status}")]
    public async Task<IActionResult> GetLeavesByStatus(string status)
    {
        var leaves = await _leaveRepository.GetLeavesByStatus(status);

        return Ok(leaves);
    }

    [HttpPut("Approve/{id}")]
    public async Task<IActionResult> ApproveLeave(int id)
    {
        var leave = await _leaveRepository.ApproveLeave(id);

        return Ok(leave);
    }

    [HttpPut("RejectLeave/{id}")]
    public async Task<IActionResult> RejectLeave(int id)
    {
        var leave = await _leaveRepository.RejectLeave(id);

        return Ok(leave);
    }

    [HttpGet("CurrentLeave")]
    public async Task<IActionResult> GetCurrentLeaves()
    {
        var leaves = await _leaveRepository.GetCurrentLeaves();

        return Ok(leaves);
    }

    [HttpGet("Statistics")]
    public async Task<IActionResult> GetDepartmentStatistics()
    {
        var statistics = await _leaveRepository.GetDepartmentStatistics();

        return Ok(statistics);
    }
}