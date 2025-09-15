namespace TaskBoard.Application.DTOs.ProjectMembers
{
    public class CreateProjectMemberDto
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
    }
}
