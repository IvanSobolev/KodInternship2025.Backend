using Demo.DAL.Enums;

namespace Demo.DAL.Dto;

public class UpdateTaskDto
{
    public Guid Id { get; set; }
    public string? Title { get; set; }
    public string? Text { get; set; }
    public Department? Department { get; set; }
}