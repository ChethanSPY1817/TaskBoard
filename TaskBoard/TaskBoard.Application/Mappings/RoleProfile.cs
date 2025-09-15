using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskBoard.Application.DTOs.ProjectMembers;
using TaskBoard.Application.DTOs.Projects; // <-- Add this
using TaskBoard.Application.DTOs.Roles;
using TaskBoard.Application.DTOs.Tags;
using TaskBoard.Application.DTOs.TaskAssignments;
using TaskBoard.Application.DTOs.TaskItems;
using TaskBoard.Application.DTOs.TaskTags;
using TaskBoard.Application.DTOs.UserProfiles;
using TaskBoard.Application.DTOs.Users;
using TaskBoard.Domain.Entities;

namespace TaskBoard.Application.Mappings
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            // Role mappings
            CreateMap<Role, RoleDto>().ReverseMap();
            CreateMap<CreateRoleDto, Role>().ReverseMap();
            CreateMap<UpdateRoleDto, Role>().ReverseMap();

            // User mappings
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<CreateUserDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id is generated in controller
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));
            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // UserProfile mappings
            CreateMap<UserProfile, UserProfileDto>().ReverseMap();
            CreateMap<CreateUserProfileDto, UserProfile>();
            CreateMap<UpdateUserProfileDto, UserProfile>();

            // ---------------------- Project mappings ----------------------
            CreateMap<Project, ProjectDto>().ReverseMap();
            CreateMap<CreateProjectDto, Project>();
            CreateMap<UpdateProjectDto, Project>();

            // ---------------------- ProjectMember mappings ----------------------
            CreateMap<ProjectMember, ProjectMemberDto>().ReverseMap();
            CreateMap<CreateProjectMemberDto, ProjectMember>();
            CreateMap<UpdateProjectMemberDto, ProjectMember>();

            // ---------------------- Tag mappings ----------------------
            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<CreateTagDto, Tag>();
            CreateMap<UpdateTagDto, Tag>();

            // ---------------------- TaskAssignment mappings ----------------------
            CreateMap<TaskAssignment, TaskAssignmentDto>().ReverseMap();
            CreateMap<CreateTaskAssignmentDto, TaskAssignment>();
            CreateMap<UpdateTaskAssignmentDto, TaskAssignment>();

            // ---------------------- TaskItems mappings ----------------------
            // Entity -> DTO
            CreateMap<TaskItem, TaskItemDto>();
            // DTO -> Entity
            CreateMap<CreateTaskItemDto, TaskItem>();
            CreateMap<UpdateTaskItemDto, TaskItem>();

            // ---------------------- TaskTags mappings ----------------------
            // Entity → DTO
            CreateMap<TaskTag, TaskTagDto>();
            // DTO → Entity
            CreateMap<CreateTaskTagDto, TaskTag>();
        }
    }
}
