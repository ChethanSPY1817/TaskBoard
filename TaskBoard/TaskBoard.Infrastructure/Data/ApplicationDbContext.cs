using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectMember> ProjectMembers { get; set; } = null!;
    public DbSet<TaskItem> Tasks { get; set; } = null!;
    public DbSet<TaskAssignment> TaskAssignments { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<TaskTag> TaskTags { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // ---------- UserProfile (1:1) where UserId is PK ----------
        builder.Entity<UserProfile>(eb =>
        {
            eb.HasKey(p => p.UserId); // UserId is primary key
            eb.Property(p => p.FullName).HasMaxLength(200);
            eb.HasOne(p => p.User)
              .WithOne(u => u.Profile)
              .HasForeignKey<UserProfile>(p => p.UserId)
              .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- Role (1:N) ----------
        builder.Entity<Role>(eb =>
        {
            eb.HasKey(r => r.Id);
            eb.Property(r => r.Name).IsRequired().HasMaxLength(50);
        });

        // ---------- User (FK to Role) ----------
        builder.Entity<User>(eb =>
        {
            eb.HasKey(u => u.Id);
            eb.Property(u => u.Email).IsRequired().HasMaxLength(200);
            eb.Property(u => u.PasswordHash).IsRequired();

            eb.HasOne(u => u.Role)
              .WithMany(r => r.Users)
              .HasForeignKey(u => u.RoleId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Project ----------
        builder.Entity<Project>(eb =>
        {
            eb.HasKey(p => p.Id);
            eb.Property(p => p.Name).IsRequired().HasMaxLength(200);

            eb.HasOne(p => p.Owner)
              .WithMany(u => u.OwnedProjects)
              .HasForeignKey(p => p.OwnerId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- ProjectMember composite key ----------
        builder.Entity<ProjectMember>(eb =>
        {
            eb.HasKey(pm => new { pm.ProjectId, pm.UserId });

            eb.HasOne(pm => pm.Project)
              .WithMany(p => p.Members)
              .HasForeignKey(pm => pm.ProjectId)
              .OnDelete(DeleteBehavior.Cascade);

            eb.HasOne(pm => pm.User)
              .WithMany(u => u.ProjectMemberships)
              .HasForeignKey(pm => pm.UserId)
              .OnDelete(DeleteBehavior.Cascade);
        });

        // ---------- TaskItem ----------
        builder.Entity<TaskItem>(eb =>
        {
            eb.HasKey(t => t.Id);
            eb.Property(t => t.Title).IsRequired().HasMaxLength(300);

            eb.HasOne(t => t.Project)
              .WithMany(p => p.Tasks)
              .HasForeignKey(t => t.ProjectId)
              .OnDelete(DeleteBehavior.Cascade);

            eb.HasOne(t => t.AssignedToUser)
              .WithMany(u => u.AssignedTasks)
              .HasForeignKey(t => t.AssignedToUserId)
              .OnDelete(DeleteBehavior.Restrict);

            eb.HasIndex(t => t.ProjectId);
            eb.HasIndex(t => t.AssignedToUserId);
            eb.HasIndex(t => t.Status);
        });

        // ---------- TaskAssignment ----------
        builder.Entity<TaskAssignment>(eb =>
        {
            eb.HasKey(a => a.Id);

            eb.HasOne(a => a.TaskItem)
              .WithMany(t => t.Assignments)
              .HasForeignKey(a => a.TaskItemId)
              .OnDelete(DeleteBehavior.Cascade);

            eb.HasOne(a => a.AssignedToUser)
              .WithMany(u => u.TaskAssignments)
              .HasForeignKey(a => a.AssignedToUserId)
              .OnDelete(DeleteBehavior.Restrict);

            eb.HasOne(a => a.AssignedByUser)
              .WithMany()
              .HasForeignKey(a => a.AssignedByUserId)
              .OnDelete(DeleteBehavior.Restrict);
        });

        // ---------- Tag ----------
        builder.Entity<Tag>(eb =>
        {
            eb.HasKey(t => t.Id);
            eb.Property(t => t.Name).IsRequired().HasMaxLength(100);
            eb.Property(t => t.ColorHex).HasMaxLength(7);
            eb.HasIndex(t => t.Name).IsUnique(false);
        });

        // ---------- TaskTag composite key ----------
        builder.Entity<TaskTag>(eb =>
        {
            eb.HasKey(tt => new { tt.TaskItemId, tt.TagId });

            eb.HasOne(tt => tt.TaskItem)
              .WithMany(t => t.TaskTags)
              .HasForeignKey(tt => tt.TaskItemId)
              .OnDelete(DeleteBehavior.Cascade);

            eb.HasOne(tt => tt.Tag)
              .WithMany(t => t.TaskTags)
              .HasForeignKey(tt => tt.TagId)
              .OnDelete(DeleteBehavior.Cascade);
        });

        // optional: seed roles with stable GUIDs (useful later)
        var adminRoleId = Guid.Parse("10000000-0000-0000-0000-000000000001");
        var managerRoleId = Guid.Parse("10000000-0000-0000-0000-000000000002");
        var developerRoleId = Guid.Parse("10000000-0000-0000-0000-000000000003");

        builder.Entity<Role>().HasData(
            new Role { Id = adminRoleId, Name = "Admin" },
            new Role { Id = managerRoleId, Name = "Manager" },
            new Role { Id = developerRoleId, Name = "Developer" }
        );
    }
}
