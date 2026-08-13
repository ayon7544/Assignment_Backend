using AssignmentSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.API.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        var admin = new User { FullName = "System Admin", Email = "admin@school.edu",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"), Role = UserRole.Admin };
        var teacher1 = new User { FullName = "Mr. Rahman", Email = "teacher1@school.edu",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"), Role = UserRole.Teacher };
        var teacher2 = new User { FullName = "Ms. Hossain", Email = "teacher2@school.edu",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Teacher@123"), Role = UserRole.Teacher };
        var student1 = new User { FullName = "Ayon Ahmed", Email = "student1@school.edu",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"), Role = UserRole.Student };
        var student2 = new User { FullName = "Rina Begum", Email = "student2@school.edu",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@123"), Role = UserRole.Student };

        db.Users.AddRange(admin, teacher1, teacher2, student1, student2);

        var class10 = new Class { Name = "Class 10 (Science)", Description = "Secondary Science" };
        var class11 = new Class { Name = "Class 11 (Commerce)", Description = "Higher Commerce" };
        db.Classes.AddRange(class10, class11);

        var math = new Subject { Name = "Mathematics", ClassId = class10.Id };
        var physics = new Subject { Name = "Physics", ClassId = class10.Id };
        var accounting = new Subject { Name = "Accounting", ClassId = class11.Id };
        db.Subjects.AddRange(math, physics, accounting);

        db.TeacherSubjects.AddRange(
            new TeacherSubject { TeacherId = teacher1.Id, SubjectId = math.Id, ClassId = class10.Id },
            new TeacherSubject { TeacherId = teacher2.Id, SubjectId = physics.Id, ClassId = class10.Id }
        );
        db.StudentClasses.AddRange(
            new StudentClass { StudentId = student1.Id, ClassId = class10.Id },
            new StudentClass { StudentId = student2.Id, ClassId = class10.Id }
        );
        await db.SaveChangesAsync();

        var a1 = new Assignment { Title = "Chapter 5 Algebra Exercises",
            Description = "Complete all exercises from Chapter 5. Show all working.",
            ClassId = class10.Id, SubjectId = math.Id, TeacherId = teacher1.Id,
            MaxMarks = 100, Deadline = DateTime.UtcNow.AddDays(7),
            Status = AssignmentStatus.Published, AllowLate = false };
        var a2 = new Assignment { Title = "Wave Motion Problems",
            Description = "Solve the 10 wave motion problems from the handout.",
            ClassId = class10.Id, SubjectId = physics.Id, TeacherId = teacher2.Id,
            MaxMarks = 50, Deadline = DateTime.UtcNow.AddDays(3),
            Status = AssignmentStatus.Published, AllowLate = true };
        var a3 = new Assignment { Title = "Kinematics Homework",
            Description = "Draft - students cannot see this yet.",
            ClassId = class10.Id, SubjectId = physics.Id, TeacherId = teacher2.Id,
            MaxMarks = 30, Deadline = DateTime.UtcNow.AddDays(10),
            Status = AssignmentStatus.Draft };
        var a4 = new Assignment { Title = "Past Due - Trigonometry",
            Description = "Tests deadline enforcement - AllowLate is false.",
            ClassId = class10.Id, SubjectId = math.Id, TeacherId = teacher1.Id,
            MaxMarks = 40, Deadline = DateTime.UtcNow.AddDays(-2),
            Status = AssignmentStatus.Published, AllowLate = false };

        db.Assignments.AddRange(a1, a2, a3, a4);
        await db.SaveChangesAsync();

        db.Submissions.AddRange(
            new Submission { AssignmentId = a1.Id, StudentId = student1.Id,
                AnswerText = "x=5, y=3. Solved using substitution method.",
                Status = SubmissionStatus.Submitted },
            new Submission { AssignmentId = a1.Id, StudentId = student2.Id,
                AnswerText = "My solutions to all exercises are below...",
                Status = SubmissionStatus.Graded, Marks = 85, Feedback = "Good work!" }
        );
        await db.SaveChangesAsync();
    }
}
