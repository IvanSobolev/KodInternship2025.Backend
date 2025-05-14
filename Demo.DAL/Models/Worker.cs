using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.DAL.Models;

public class Worker
{
    [Key]
    public long TelegramId { get; set; }

    [MaxLength(200)]
    public string FullName { get; set; }
    
    [MaxLength(100)]
    public Department Department { get; set; } 

    [MaxLength(100)]
    public string? TelegramUsername { get; set; }
    
    public ProjectTask? AssignedTask { get; set; }
}