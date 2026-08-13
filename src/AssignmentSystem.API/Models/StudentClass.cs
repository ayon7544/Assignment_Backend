namespace AssignmentSystem.API.Models;

public class StudentClass
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid ClassId { get; set; }
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    public User Student { get; set; } = null!;
    public Class Class { get; set; } = null!;
}