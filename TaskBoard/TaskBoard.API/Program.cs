using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaskBoard.Application.Services.Auth;
using TaskBoard.Domain.Entities;
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

// ------------------- DbContext -------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ------------------- Token Service -------------------
builder.Services.AddScoped<ITokenService, TokenService>();

// ------------------- Authentication (JWT) -------------------
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
        };
    });



// ------------------- Authorization -------------------
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireSuperAdmin", policy => policy.RequireRole("SuperAdmin"));
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireManager", policy => policy.RequireRole("Manager"));
    options.AddPolicy("RequireDeveloper", policy => policy.RequireRole("Developer"));

    // Custom example: ProjectOwnerOrManager
    options.AddPolicy("ProjectOwnerOrManager", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Manager") ||
            context.User.HasClaim(c => c.Type == "ProjectOwner" && c.Value == "true")));
});

// ------------------- Repositories -------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();
builder.Services.AddScoped<ITaskAssignmentRepository, TaskAssignmentRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITaskTagRepository, TaskTagRepository>();
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();

// ------------------- AutoMapper -------------------
builder.Services.AddAutoMapper(typeof(TaskBoard.Application.Mappings.RoleProfile).Assembly);

// ------------------- Controllers + Swagger -------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskBoard API", Version = "v1" });

    // JWT in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {your token}'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ---------------- Apply Migrations and Seed Once ----------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    // Apply migrations
    await dbContext.Database.MigrateAsync();

    // Seed only if DB is empty
    await DbSeeder.SeedOnceAsync(dbContext);
}

// ------------------- Middleware -------------------
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
