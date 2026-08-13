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
public class AssignmentServiceTests
{
    private readonly Mock<IAssignmentRepository> _ar = new();
    private readonly Mock<ITeacherSubjectRepository> _tr = new();
    private readonly Mock<IStudentClassRepository> _sr = new();
    private readonly AssignmentService _s;
    public AssignmentServiceTests() { _s = new AssignmentService(_ar.Object, _tr.Object, _sr.Object); }

    [Fact]
    public async Task Create_WhenTeacherNotAssigned_ThrowsForbidden()
    {
        var tid = Guid.NewGuid();
        var req = new CreateAssignmentRequest("T","D",Guid.NewGuid(),Guid.NewGuid(),100,DateTime.UtcNow.AddDays(1));
        _tr.Setup(x => x.ExistsAsync(tid, req.SubjectId, req.ClassId)).ReturnsAsync(false);
        var act = () => _s.CreateAsync(req, tid);
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Create_WhenDeadlineInPast_ThrowsBusinessRule()
    {
        var tid = Guid.NewGuid(); var cid = Guid.NewGuid(); var sid = Guid.NewGuid();
        var req = new CreateAssignmentRequest("T","D",cid,sid,100,DateTime.UtcNow.AddDays(-1));
        _tr.Setup(x => x.ExistsAsync(tid, sid, cid)).ReturnsAsync(true);
        var act = () => _s.CreateAsync(req, tid);
        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task Publish_WhenOtherTeacher_ThrowsForbidden()
    {
        var t1 = Guid.NewGuid(); var t2 = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(t2, Guid.NewGuid(), Guid.NewGuid());
        _ar.Setup(x => x.GetByIdAsync(a.Id)).ReturnsAsync(a);
        var act = () => _s.PublishAsync(a.Id, t1);
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task Delete_WhenPublished_ThrowsBusinessRule()
    {
        var tid = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(tid, Guid.NewGuid(), Guid.NewGuid(), AssignmentStatus.Published);
        _ar.Setup(x => x.GetByIdAsync(a.Id)).ReturnsAsync(a);
        var act = () => _s.DeleteAsync(a.Id, tid);
        await act.Should().ThrowAsync<BusinessRuleException>();
    }

    [Fact]
    public async Task Delete_WhenOtherTeacher_ThrowsForbidden()
    {
        var t1 = Guid.NewGuid(); var t2 = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(t2, Guid.NewGuid(), Guid.NewGuid(), AssignmentStatus.Draft);
        _ar.Setup(x => x.GetByIdAsync(a.Id)).ReturnsAsync(a);
        var act = () => _s.DeleteAsync(a.Id, t1);
        await act.Should().ThrowAsync<ForbiddenException>();
    }

    [Fact]
    public async Task GetForStudent_ReturnsOnlyPublishedForTheirClass()
    {
        var sid = Guid.NewGuid(); var cid = Guid.NewGuid();
        var sc = TestDataBuilder.BuildStudentClass(sid, cid);
        var a = TestDataBuilder.BuildAssignment(Guid.NewGuid(), cid, Guid.NewGuid(), AssignmentStatus.Published);
        a.Class = TestDataBuilder.BuildClass(cid);
        _sr.Setup(x => x.GetByStudentIdAsync(sid)).ReturnsAsync(new List<StudentClass> { sc });
        _ar.Setup(x => x.GetPublishedByClassAsync(cid)).ReturnsAsync(new List<Assignment> { a });
        var r = await _s.GetPublishedForStudentAsync(sid);
        r.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetForTeacher_ReturnsOnlyOwn()
    {
        var tid = Guid.NewGuid();
        var a = TestDataBuilder.BuildAssignment(tid, Guid.NewGuid(), Guid.NewGuid());
        _ar.Setup(x => x.GetByTeacherAsync(tid)).ReturnsAsync(new List<Assignment> { a });
        var r = await _s.GetByTeacherAsync(tid);
        r.Should().HaveCount(1);
    }
}
