using System.Security.Claims;
using AssignmentSystem.API.DTOs.Requests;
using AssignmentSystem.API.DTOs.Responses;
using AssignmentSystem.API.Repositories.Interfaces;
using AssignmentSystem.API.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/teacher")]
[Tags("Teacher")]
[Authorize(Roles = "Teacher")]
public class TeacherController : ControllerBase
{
    private readonly IAssignmentService _assignments;
    private readonly ISubmissionService _submissions;
    private readonly ITeacherSubjectRepository _teacherSubjects;
    private readonly IValidator<CreateAssignmentRequest> _createAssignmentValidator;
    private readonly IValidator<GradeSubmissionRequest> _gradeSubmissionValidator;

    public TeacherController(
        IAssignmentService assignments,
        ISubmissionService submissions,
        ITeacherSubjectRepository teacherSubjects,
        IValidator<CreateAssignmentRequest> createAssignmentValidator,
        IValidator<GradeSubmissionRequest> gradeSubmissionValidator)
    {
        _assignments = assignments;
        _submissions = submissions;
        _teacherSubjects = teacherSubjects;
        _createAssignmentValidator = createAssignmentValidator;
        _gradeSubmissionValidator = gradeSubmissionValidator;
    }

    private Guid TeacherId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("subjects")]
    public async Task<IActionResult> GetMySubjects()
    {
        var teacherSubjects =
            await _teacherSubjects.GetByTeacherIdAsync(TeacherId);

        var result = teacherSubjects
            .Select(ts => new SubjectResponse(
                ts.SubjectId,
                ts.Subject.Name,
                ts.ClassId,
                ts.Class.Name,
                ts.Subject.CreatedAt
            ))
            .OrderBy(s => s.ClassName)
            .ThenBy(s => s.Name)
            .ToList();

        return Ok(
            ApiResponse<List<SubjectResponse>>.Ok(result)
        );
    }

    [HttpGet("assignments")]
    public async Task<IActionResult> GetMyAssignments()
    {
        var result =
            await _assignments.GetByTeacherAsync(TeacherId);

        return Ok(
            ApiResponse<List<AssignmentResponse>>.Ok(result)
        );
    }

    [HttpPost("assignments")]
    public async Task<IActionResult> Create(
        [FromBody] CreateAssignmentRequest req)
    {
        var validation =
            await _createAssignmentValidator.ValidateAsync(req);

        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(error => error.ErrorMessage)
                .ToList();

            return BadRequest(
                ApiResponse<object>.Fail(
                    "Validation failed",
                    400,
                    errors
                )
            );
        }

        var result =
            await _assignments.CreateAsync(req, TeacherId);

        return StatusCode(
            201,
            ApiResponse<AssignmentResponse>.Created(
                result,
                "Assignment created"
            )
        );
    }

    [HttpPut("assignments/{id}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateAssignmentRequest req)
    {
        var result =
            await _assignments.UpdateAsync(
                id,
                req,
                TeacherId
            );

        return Ok(
            ApiResponse<AssignmentResponse>.Ok(
                result,
                "Assignment updated"
            )
        );
    }

    [HttpDelete("assignments/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _assignments.DeleteAsync(
            id,
            TeacherId
        );

        return Ok(
            ApiResponse.OkNoData(
                "Assignment deleted"
            )
        );
    }

    [HttpPatch("assignments/{id}/publish")]
    public async Task<IActionResult> Publish(Guid id)
    {
        var result =
            await _assignments.PublishAsync(
                id,
                TeacherId
            );

        return Ok(
            ApiResponse<AssignmentResponse>.Ok(
                result,
                "Assignment published"
            )
        );
    }

    [HttpGet("assignments/{id}/submissions")]
    public async Task<IActionResult> GetSubmissions(
        Guid id)
    {
        var result =
            await _submissions.GetByAssignmentAsync(
                id,
                TeacherId
            );

        return Ok(
            ApiResponse<List<SubmissionResponse>>.Ok(
                result
            )
        );
    }

    [HttpPatch("submissions/{id}/grade")]
    public async Task<IActionResult> Grade(
        Guid id,
        [FromBody] GradeSubmissionRequest req)
    {
        var validation =
            await _gradeSubmissionValidator.ValidateAsync(req);

        if (!validation.IsValid)
        {
            var errors = validation.Errors
                .Select(error => error.ErrorMessage)
                .ToList();

            return BadRequest(
                ApiResponse<object>.Fail(
                    "Validation failed",
                    400,
                    errors
                )
            );
        }

        var result =
            await _submissions.GradeAsync(
                id,
                req,
                TeacherId
            );

        return Ok(
            ApiResponse<SubmissionDetailResponse>.Ok(
                result,
                "Submission graded"
            )
        );
    }
}