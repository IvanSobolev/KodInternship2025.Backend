using Demo.DAL.Enums;

namespace Demo.DAL.Dto;

public class UpdateTaskDto (Guid id, string? title = null, string? text = null, Department? department = null)
{
    public Guid Id { get; set; }
    public string? Title { get; set; } = title;
    public string? Text { get; set; } = text;
    public Department? Department { get; set; } = department;
}