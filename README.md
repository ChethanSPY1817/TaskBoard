TaskBoard.sln
/src
  /TaskBoard.Api              <- Web API (Controllers, Middleware, Logging)
      ├── Controllers
      │   ├── ProjectsController.cs
      │   ├── UsersController.cs
      │   ├── RolesController.cs
      │   ├── TagsController.cs
      │   ├── TaskItemsController.cs
      │   ├── TaskAssignmentsController.cs
      │   ├── ProjectMembersController.cs
      │   ├── TaskTagsController.cs
      │   └── UserProfilesController.cs
      │
      ├── Middleware
      │   ├── RequestLoggingMiddleware.cs
      │   ├── ErrorHandlingMiddleware.cs
      │
      └── Logs
          ├── all.txt
  /TaskBoard.Domain           <- Entities, Enums, Interfaces, DTOs
      ├── DTOS                <- DTOs (Projects, Users, Roles, Tags, TaskItems, TaskAssignments, ProjectMembers, TaskTags, UserProfiles)
      │   ├── Projects
      │   │   ├── ProjectDto.cs
      │   │   ├── CreateProjectDto.cs
      │   │   └── UpdateProjectDto.cs
      │   │
      │   ├── Users
      │   │   ├── UserDto.cs
      │   │   ├── CreateUserDto.cs
      │   │   └── UpdateUserDto.cs
      │   │
      │   ├── Roles
      │   │   ├── RoleDto.cs
      │   │   ├── CreateRoleDto.cs
      │   │   └── UpdateRoleDto.cs
      │   │
      │   ├── Tags
      │   │   ├── TagDto.cs
      │   │   ├── CreateTagDto.cs
      │   │   └── UpdateTagDto.cs
      │   │
      │   ├── TaskItems
      │   │   ├── TaskItemDto.cs
      │   │   ├── CreateTaskItemDto.cs
      │   │   └── UpdateTaskItemDto.cs
      │   │
      │   ├── TaskAssignments
      │   │   ├── TaskAssignmentDto.cs
      │   │   ├── CreateTaskAssignmentDto.cs
      │   │   └── UpdateTaskAssignmentDto.cs
      │   │
      │   ├── ProjectMembers
      │   │   ├── ProjectMemberDto.cs
      │   │   ├── CreateProjectMemberDto.cs
      │   │   └── UpdateProjectMemberDto.cs
      │   │
      │   ├── TaskTags
      │   │   ├── TaskTagDto.cs
      │   │   ├── CreateTaskTagDto.cs
      │   │   └── UpdateTaskTagDto.cs
      │   │
      │   └── UserProfiles
      │       ├── UserProfileDto.cs
      │       ├── CreateUserProfileDto.cs
      │       └── UpdateUserProfileDto.cs
  /TaskBoard.Infrastructure   <- EF Core DbContext, Repositories, Migrations, Identity
  /TaskBoard.Application      <- Services, Business Logic, DTO Validators
/tests
  /TaskBoard.UnitTests
/tools
  /docker
