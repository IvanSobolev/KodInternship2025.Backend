using Demo.DAL.Enums;

namespace Demo.DAL.Dto;

public class UpdateTaskDto (Guid id, string? title = null, string? text = null, Department? department = null)
{
    private Guid Id { get; set; }
    private string? Title { get; set; } = title;
    private string? Text { get; set; } = text;
    private Department? Department { get; set; } = department;
}