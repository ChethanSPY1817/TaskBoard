using Microsoft.EntityFrameworkCore;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Infrastructure.Data
{
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

        public DbSet<PlainTextPassword> PlainTextPasswords { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ---------------- 1:1 UserProfile ----------------
            builder.Entity<UserProfile>()
                .HasKey(p => p.UserId);

            builder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- Roles ----------------
            builder.Entity<Role>()
                .HasKey(r => r.Id);

            builder.Entity<Role>()
                .Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(50);

            // ---------------- Users ----------------
            builder.Entity<User>()
                .HasKey(u => u.Id);

            builder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired();

            builder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- Projects ----------------
            builder.Entity<Project>()
                .HasKey(p => p.Id);

            builder.Entity<Project>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Entity<Project>()
                .HasOne(p => p.Owner)
                .WithMany(u => u.OwnedProjects)
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- ProjectMembers ----------------
            builder.Entity<ProjectMember>()
                .HasKey(pm => new { pm.ProjectId, pm.UserId });

            builder.Entity<ProjectMember>()
                .HasOne(pm => pm.Project)
                .WithMany(p => p.Members)
                .HasForeignKey(pm => pm.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ProjectMember>()
                .HasOne(pm => pm.User)
                .WithMany(u => u.ProjectMemberships)
                .HasForeignKey(pm => pm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ---------------- TaskItem ----------------
            builder.Entity<TaskItem>()
                .HasKey(t => t.Id);

            builder.Entity<TaskItem>()
                .Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(300);

            builder.Entity<TaskItem>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskItem>()
                .HasOne(t => t.AssignedToUser)
                .WithMany(u => u.AssignedTasks)
                .HasForeignKey(t => t.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskItem>().HasIndex(t => t.ProjectId);
            builder.Entity<TaskItem>().HasIndex(t => t.AssignedToUserId);
            builder.Entity<TaskItem>().HasIndex(t => t.Status);

            // ---------------- TaskAssignment ----------------
            builder.Entity<TaskAssignment>()
                .HasKey(a => a.Id);

            builder.Entity<TaskAssignment>()
                .HasOne(a => a.TaskItem)
                .WithMany(t => t.Assignments)
                .HasForeignKey(a => a.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskAssignment>()
                .HasOne(a => a.AssignedToUser)
                .WithMany(u => u.TaskAssignments)
                .HasForeignKey(a => a.AssignedToUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskAssignment>()
                .HasOne(a => a.AssignedByUser)
                .WithMany()
                .HasForeignKey(a => a.AssignedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- Tag ----------------
            builder.Entity<Tag>()
                .HasKey(t => t.Id);

            builder.Entity<Tag>()
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<Tag>()
                .Property(t => t.ColorHex)
                .HasMaxLength(7);

            builder.Entity<Tag>()
                .HasIndex(t => t.Name);

            // ---------------- TaskTag ----------------
            builder.Entity<TaskTag>()
                .HasKey(tt => new { tt.TaskItemId, tt.TagId });

            builder.Entity<TaskTag>()
                .HasOne(tt => tt.TaskItem)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TaskTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TaskTags)
                .HasForeignKey(tt => tt.TagId)
                .OnDelete(DeleteBehavior.Cascade);

                builder.Entity<PlainTextPassword>()
           .HasOne(p => p.User)
           .WithMany() // Optional: add navigation property in User if desired
           .HasForeignKey(p => p.UserId)
           .OnDelete(DeleteBehavior.Cascade);
            }
    }
}
