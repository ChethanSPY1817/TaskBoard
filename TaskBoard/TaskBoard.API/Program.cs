using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Infrastructure.Data;
using TaskBoard.Infrastructure.Repositories;
using TaskBoard.Infrastructure.Repositories.P;
using TaskBoard.Infrastructure.Repositories.ProjectMemberRepository;
using TaskBoard.Infrastructure.Repositories.ProjectRepository;
using TaskBoard.Infrastructure.Repositories.RoleRepository;
using TaskBoard.Infrastructure.Repositories.TagRepository;
using TaskBoard.Infrastructure.Repositories.TaskAssignmentRepository;
using TaskBoard.Infrastructure.Repositories.TaskItemRepository;
using TaskBoard.Infrastructure.Repositories.TaskTagRepository;
using TaskBoard.Infrastructure.Repositories.UserProfileRepository;
using TaskBoard.Infrastructure.Repositories.UserRepository;

var builder = WebApplication.CreateBuilder(args);

// ---------- Configure DbContext ----------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------- Add Identity ----------
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// ---------- Register Repositories ----------
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IProjectRepository, ProjectRepository>();
builder.Services.AddTransient<IProjectMemberRepository, ProjectMemberRepository>();
builder.Services.AddTransient<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddTransient<ITaskAssignmentRepository, TaskAssignmentRepository>();
builder.Services.AddTransient<ITagRepository, TagRepository>();
builder.Services.AddTransient<ITaskTagRepository, TaskTagRepository>();
builder.Services.AddTransient<IUserProfileRepository, UserProfileRepository>();

// ---------- Controllers + Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ---------- Swagger only in Development ----------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
