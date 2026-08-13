namespace AssignmentSystem.API.Models;

public class Submission
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AssignmentId { get; set; }
    public Guid StudentId { get; set; }
    public string? AnswerText { get; set; }
    public string? FileUrl { get; set; }
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int? Marks { get; set; }
    public string? Feedback { get; set; }
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;
    public bool IsLate { get; set; } = false;

    public Assignment Assignment { get; set; } = null!;
    public User Student { get; set; } = null!;
}

public enum SubmissionStatus { Submitted, Graded, Late, Rejected }
