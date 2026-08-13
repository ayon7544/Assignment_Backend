using System.Security.Claims;
using AssignmentSystem.API.DTOs.Requests;
using AssignmentSystem.API.DTOs.Responses;
using AssignmentSystem.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/student")]
[Tags("Student")]
[Authorize(Roles = "Student")]
public class StudentController : ControllerBase
{
    private readonly IAssignmentService _assignments;
    private readonly ISubmissionService _submissions;

    public StudentController(IAssignmentService assignments, ISubmissionService submissions)
    {
        _assignments = assignments;
        _submissions = submissions;
    }

    private Guid StudentId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("assignments")]
    public async Task<IActionResult> GetMyAssignments()
    {
        var result = await _assignments.GetPublishedForStudentAsync(StudentId);
        return Ok(ApiResponse<List<AssignmentResponse>>.Ok(result));
    }

    [HttpGet("assignments/{id}")]
    public async Task<IActionResult> GetAssignment(Guid id)
    {
        var result = await _assignments.GetByIdForStudentAsync(id, StudentId);
        return Ok(ApiResponse<AssignmentResponse>.Ok(result));
    }

    [HttpPost("assignments/{id}/submit")]
    public async Task<IActionResult> Submit(Guid id, [FromBody] CreateSubmissionRequest req)
    {
        var result = await _submissions.SubmitAsync(id, req, StudentId);
        return StatusCode(201, ApiResponse<SubmissionResponse>.Created(result, "Submitted successfully"));
    }

    [HttpPut("submissions/{id}")]
    public async Task<IActionResult> UpdateSubmission(Guid id, [FromBody] UpdateSubmissionRequest req)
    {
        var result = await _submissions.UpdateAsync(id, req, StudentId);
        return Ok(ApiResponse<SubmissionResponse>.Ok(result, "Submission updated"));
    }

    [HttpGet("submissions")]
    public async Task<IActionResult> GetMySubmissions()
    {
        var result = await _submissions.GetByStudentAsync(StudentId);
        return Ok(ApiResponse<List<SubmissionResponse>>.Ok(result));
    }
}
