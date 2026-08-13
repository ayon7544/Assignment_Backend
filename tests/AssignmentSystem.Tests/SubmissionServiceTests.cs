using Xunit;
using AssignmentSystem.API.DTOs.Requests;
using AssignmentSystem.API.Middleware;
using AssignmentSystem.API.Models;
using AssignmentSystem.API.Repositories.Interfaces;
using AssignmentSystem.API.Services;
using AssignmentSystem.Tests.Helpers;
using FluentAssertions;
using Moq;
namespace AssignmentSystem.Tests.Services;
public class SubmissionServiceTests
{
    private readonly Mock<ISubmissionRepository> _sr = new();
    private readonly Mock<IAssignmentRepository> _ar = new();
    private readonly Mock<IStudentClassRepository> _cr = new();
    private readonly SubmissionService _s;
    public SubmissionServiceTests() { _s = new SubmissionService(_sr.Object, _ar.Object, _cr.Object); }

    [Fact]
    public async Task Submit_WhenDeadlinePassed_NoAllowLate_ThrowsDeadline()
    {
        var sid = Guid.NewGuid(); var cid = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(Guid.NewGuid(), cid, Guid.NewGuid(), AssignmentStatus.Published, false);
        a.Deadline = DateTime.UtcNow.AddDays(-1); a.Class = TestDataBuilder.BuildClass(cid);
        _ar.Setup(x => x.GetByIdAsync(a.Id)).ReturnsAsync(a);
        _cr.Setup(x => x.ExistsAsync(sid, cid)).ReturnsAsync(true);
        _sr.Setup(x => x.GetByAssignmentAndStudentAsync(a.Id, sid)).ReturnsAsync((Submission?)null);
        var act = () => _s.SubmitAsync(a.Id, new CreateSubmissionRequest("a", null), sid);
        await act.Should().ThrowAsync<DeadlineException>();
    }

    [Fact]
    public async Task Submit_WhenDeadlinePassed_AllowLate_MarksLate()
    {
        var sid = Guid.NewGuid(); var cid = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(Guid.NewGuid(), cid, Guid.NewGuid(), AssignmentStatus.Published, true);
        a.Deadline = DateTime.UtcNow.AddDays(-1); a.Class = TestDataBuilder.BuildClass(cid);
        _ar.Setup(x => x.GetByIdAsync(a.Id)).ReturnsAsync(a);
        _cr.Setup(x => x.ExistsAsync(sid, cid)).ReturnsAsync(true);
        _sr.Setup(x => x.GetByAssignmentAndStudentAsync(a.Id, sid)).ReturnsAsync((Submission?)null);
        _sr.Setup(x => x.CreateAsync(It.IsAny<Submission>())).ReturnsAsync((Submission s) => s);
        var r = await _s.SubmitAsync(a.Id, new CreateSubmissionRequest("a", null), sid);
        r.IsLate.Should().BeTrue();
        r.Status.Should().Be("Late");
    }

    [Fact]
    public async Task Submit_WhenAlreadySubmitted_ThrowsDuplicate()
    {
        var sid = Guid.NewGuid(); var cid = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(Guid.NewGuid(), cid, Guid.NewGuid(), AssignmentStatus.Published);
        a.Class = TestDataBuilder.BuildClass(cid);
        var ex = TestDataBuilder.BuildSubmission(a.Id, sid);
        _ar.Setup(x => x.GetByIdAsync(a.Id)).ReturnsAsync(a);
        _cr.Setup(x => x.ExistsAsync(sid, cid)).ReturnsAsync(true);
        _sr.Setup(x => x.GetByAssignmentAndStudentAsync(a.Id, sid)).ReturnsAsync(ex);
        var act = () => _s.SubmitAsync(a.Id, new CreateSubmissionRequest("a", null), sid);
        await act.Should().ThrowAsync<DuplicateException>();
    }

    [Fact]
    public async Task Submit_WhenNotPublished_ThrowsNotFound()
    {
        var sid = Guid.NewGuid(); var cid = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(Guid.NewGuid(), cid, Guid.NewGuid(), AssignmentStatus.Draft);
        a.Class = TestDataBuilder.BuildClass(cid);
        _ar.Setup(x => x.GetByIdAsync(a.Id)).ReturnsAsync(a);
        var act = () => _s.SubmitAsync(a.Id, new CreateSubmissionRequest("a", null), sid);
        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Submit_WhenNotInClass_ThrowsForbidden()
    {
        var sid = Guid.NewGuid(); var cid = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(Guid.NewGuid(), cid, Guid.NewGuid(), AssignmentStatus.Published);
        a.Class = TestDataBuilder.BuildClass(cid);
        _ar.Setup(x => x.GetByIdAsync(a.Id)).ReturnsAsync(a);
        _cr.Setup(x => x.ExistsAsync(sid, cid)).ReturnsAsync(false);
        _sr.Setup(x => x.GetByAssignmentAndStudentAsync(a.Id, sid)).ReturnsAsync((Submission?)null);
        var act = () => _s.SubmitAsync(a.Id, new CreateSubmissionRequest("a", null), sid);
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Update_AfterDeadline_ThrowsDeadline()
    {
        var sid = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), AssignmentStatus.Published);
        a.Deadline = DateTime.UtcNow.AddDays(-1);
        var sub = TestDataBuilder.BuildSubmission(a.Id, sid); sub.Assignment = a;
        _sr.Setup(x => x.GetByIdAsync(sub.Id)).ReturnsAsync(sub);
        var act = () => _s.UpdateAsync(sub.Id, new UpdateSubmissionRequest("u", null), sid);
        await act.Should().ThrowAsync<DeadlineException>();
    }

    [Fact]
    public async Task Grade_WhenMarksExceedMax_ThrowsBusinessRule()
    {
        var tid = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(tid, Guid.NewGuid(), Guid.NewGuid());
        a.MaxMarks = 50;
        var sub = TestDataBuilder.BuildSubmission(a.Id, Guid.NewGuid()); sub.Assignment = a;
        _sr.Setup(x => x.GetByIdAsync(sub.Id)).ReturnsAsync(sub);
        var act = () => _s.GradeAsync(sub.Id, new GradeSubmissionRequest(60, "G", "Graded"), tid);
        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task Grade_WhenOtherTeacher_ThrowsForbidden()
    {
        var t1 = Guid.NewGuid(); var t2 = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(t2, Guid.NewGuid(), Guid.NewGuid());
        var sub = TestDataBuilder.BuildSubmission(a.Id, Guid.NewGuid()); sub.Assignment = a;
        _sr.Setup(x => x.GetByIdAsync(sub.Id)).ReturnsAsync(sub);
        var act = () => _s.GradeAsync(sub.Id, new GradeSubmissionRequest(80, "G", "Graded"), t1);
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}
