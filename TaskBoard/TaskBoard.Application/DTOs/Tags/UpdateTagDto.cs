namespace TaskBoard.Application.DTOs.Tags
{
    public class UpdateTagDto
    {
        public string Name { get; set; } = default!;
        public string ColorHex { get; set; } = "#000000";
    }
}
