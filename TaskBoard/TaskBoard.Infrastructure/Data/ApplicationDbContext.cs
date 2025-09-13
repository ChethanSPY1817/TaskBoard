using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    //  DbSets
    public DbSet<User> Users { get; set; } = default!;
    public DbSet<UserProfile> UserProfiles { get; set; } = default!;
    public DbSet<Project> Projects { get; set; } = default!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = default!;
    public DbSet<TaskItem> Tasks { get; set; } = default!;
    public DbSet<TaskAssignment> TaskAssignments { get; set; } = default!;
    public DbSet<Tag> Tags { get; set; } = default!;
    public DbSet<TaskTag> TaskTags { get; set; } = default!;
    public DbSet<Role> Roles { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // User  Profile (1:1)
        builder.Entity<User>()
            .HasOne(u => u.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId);

        // User  Role (1:N)
        builder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Project  Owner (1:N)
        builder.Entity<Project>()
            .HasOne<User>()
            .WithMany(u => u.OwnedProjects)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // ProjectMember composite key
        builder.Entity<ProjectMember>()
            .HasKey(pm => new { pm.ProjectId, pm.UserId });

        // Task  Project (1:N)
        builder.Entity<TaskItem>()
            .HasOne(p => p.Project)
            .WithMany(t => t.Tasks)
            .HasForeignKey(t => t.ProjectId);

        // Task  User (AssignedTo)
        builder.Entity<TaskItem>()
            .HasOne(t => t.AssignedToUser)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // TaskAssignment ↔ Task ↔ User
        builder.Entity<TaskAssignment>()
            .HasOne(ta => ta.TaskItem)
            .WithMany(t => t.Assignments)
            .HasForeignKey(ta => ta.TaskItemId);

        builder.Entity<TaskAssignment>()
            .HasOne(ta => ta.AssignedToUser)
            .WithMany(u => u.TaskAssignments)
            .HasForeignKey(ta => ta.AssignedToUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<TaskAssignment>()
            .HasOne(ta => ta.AssignedByUser)
            .WithMany()
            .HasForeignKey(ta => ta.AssignedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // TaskTag composite key
        builder.Entity<TaskTag>()
            .HasKey(tt => new { tt.TaskItemId, tt.TagId });

        //  Seed Roles
        builder.Entity<Role>().HasData(
            new Role { Id = Guid.NewGuid(), Name = "Admin" },
            new Role { Id = Guid.NewGuid(), Name = "Manager" },
            new Role { Id = Guid.NewGuid(), Name = "Developer" }
        );
    }
}
