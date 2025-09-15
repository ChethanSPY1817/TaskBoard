using BCrypt.Net;
using TaskBoard.Domain.Entities;
using TaskBoard.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedOnceAsync(ApplicationDbContext context)
    {
        // If roles already exist, skip seeding
        if (context.Roles.Any()) return;

        // ---------- Roles ----------
        var roles = new List<Role>
        {
            new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000000"), Name = "SuperAdmin" },
            new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "Admin" },
            new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "Manager" },
            new Role { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "Developer" },
        };

        context.Roles.AddRange(roles);
        await context.SaveChangesAsync();

        // ---------- SuperAdmin User ----------
        var superAdminUser = new User
        {
            Id = Guid.Parse("20000000-0000-0000-0000-000000000000"),
            Email = "superadmin@taskboard.com",
            RoleId = roles.First(r => r.Name == "SuperAdmin").Id,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("SuperAdmin@123")
        };

        context.Users.Add(superAdminUser);
        await context.SaveChangesAsync();
    }
}
