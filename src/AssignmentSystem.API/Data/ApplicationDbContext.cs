using AssignmentSystem.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AssignmentSystem.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<TeacherSubject> TeacherSubjects => Set<TeacherSubject>();
    public DbSet<StudentClass> StudentClasses => Set<StudentClass>();
    public DbSet<Assignment> Assignments => Set<Assignment>();
    public DbSet<Submission> Submissions => Set<Submission>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>(e => {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Role).HasConversion<string>();
        });

        builder.Entity<TeacherSubject>(e => {
            e.HasIndex(ts => new { ts.TeacherId, ts.SubjectId, ts.ClassId }).IsUnique();
        });

        builder.Entity<StudentClass>(e => {
            e.HasIndex(sc => new { sc.StudentId, sc.ClassId }).IsUnique();
        });

        builder.Entity<Assignment>(e => {
            e.Property(a => a.Status).HasConversion<string>();
        });

        builder.Entity<Submission>(e => {
            e.HasIndex(s => new { s.AssignmentId, s.StudentId }).IsUnique();
            e.Property(s => s.Status).HasConversion<string>();
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
        {
            if (entry.Entity is User u) u.UpdatedAt = DateTime.UtcNow;
            if (entry.Entity is Assignment a) a.UpdatedAt = DateTime.UtcNow;
            if (entry.Entity is Submission s) s.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(ct);
    }
}