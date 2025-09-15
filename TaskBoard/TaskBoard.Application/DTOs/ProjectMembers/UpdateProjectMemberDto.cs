namespace TaskBoard.Application.DTOs.ProjectMembers
{
    public class UpdateProjectMemberDto
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
    }
}
