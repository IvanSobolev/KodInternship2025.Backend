using Demo.DAL.Enums;

namespace Demo.DAL.Dto;

public class AddProjectTaskDto (string title, string text, Department department)
{
    public string Title { get; set; } = title;
    public string Text { get; set; } = text;
    public Department Department { get; set; } = department;
}