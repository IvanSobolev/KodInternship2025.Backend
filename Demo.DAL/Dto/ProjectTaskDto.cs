using Demo.DAL.Models;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace Demo.DAL.Dto;

public class ProjectTaskDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Title { get; set; }

    public string Text { get; set; }

    public TaskStatus Status { get; set; }

    public Department Department { get; set; } 
    
    public Guid? AssignedWorkerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}