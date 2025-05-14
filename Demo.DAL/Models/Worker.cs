using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.DAL.Models;

public class Worker
{
    [Key] // Первичный ключ
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [MaxLength(200)]
    public string FullName { get; set; }

    [Required]
    public long TelegramUserId { get; set; }

    [MaxLength(100)]
    public string? TelegramUsername { get; set; }

    [Required]
    [MaxLength(100)]
    public Department Department { get; set; } 

    public virtual ICollection<ProjectTask> AssignedTasks { get; set; } = new List<ProjectTask>();
}