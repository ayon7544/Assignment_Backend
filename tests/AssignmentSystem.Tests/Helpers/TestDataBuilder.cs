using AssignmentSystem.API.Models;
namespace AssignmentSystem.Tests.Helpers;
public static class TestDataBuilder
{
    public static User BuildTeacher(Guid? id = null) => new() { Id = id ?? Guid.NewGuid(), Email = "teacher@test.com", PasswordHash = "hash", FullName = "Test Teacher", Role = UserRole.Teacher, IsActive = true };
    public static User BuildStudent(Guid? id = null) => new() { Id = id ?? Guid.NewGuid(), Email = "student@test.com", PasswordHash = "hash", FullName = "Test Student", Role = UserRole.Student, IsActive = true };
    public static Class BuildClass(Guid? id = null) => new() { Id = id ?? Guid.NewGuid(), Name = "Test Class", IsActive = true };
    public static Subject BuildSubject(Guid? classId = null) => new() { Id = Guid.NewGuid(), Name = "Test Subject", ClassId = classId ?? Guid.NewGuid() };
    public static TeacherSubject BuildTeacherSubject(Guid tid, Guid sid, Guid cid) => new() { TeacherId = tid, SubjectId = sid, ClassId = cid };
    public static StudentClass BuildStudentClass(Guid sid, Guid cid) => new() { StudentId = sid, ClassId = cid };
    public static Assignment BuildAssignment(Guid tid, Guid cid, Guid sid, AssignmentStatus st = AssignmentStatus.Draft, bool al = false) => new() { Id = Guid.NewGuid(), Title = "Test", Description = "D", TeacherId = tid, ClassId = cid, SubjectId = sid, MaxMarks = 100, Deadline = DateTime.UtcNow.AddDays(7), Status = st, AllowLate = al };
    public static Submission BuildSubmission(Guid aid, Guid sid, SubmissionStatus st = SubmissionStatus.Submitted) => new() { Id = Guid.NewGuid(), AssignmentId = aid, StudentId = sid, AnswerText = "A", Status = st };
}
