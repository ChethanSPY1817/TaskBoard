namespace TaskBoard.Application.DTOs.Tags
{
    public class CreateTagDto
    {
        public string Name { get; set; } = default!;
        public string ColorHex { get; set; } = "#000000";
    }
}
