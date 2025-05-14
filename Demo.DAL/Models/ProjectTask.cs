using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Demo.DAL.Enums;
using TaskStatus = Demo.DAL.Enums.TaskStatus;

namespace Demo.DAL.Models;

public class ProjectTask
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(250)]
    public string Title { get; set; }

    [Required]
    public string Text { get; set; }

    [Required]
    public TaskStatus Status { get; set; }

    [Required]
    public Department Department { get; set; } 
    
    public long? AssignedWorkerId { get; set; }
    
    [ForeignKey("AssignedWorkerId")]
    public Worker? AssignedWorker { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}