using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskBoard.Infrastructure.Data;
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
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<ITaskAssignmentRepository, TaskAssignmentRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITaskTagRepository, TaskTagRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

// ---------- Controllers + Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add AutoMapper
// Correct: pass the assembly containing your profiles
builder.Services.AddAutoMapper(typeof(TaskBoard.Application.Mappings.RoleProfile).Assembly);

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
