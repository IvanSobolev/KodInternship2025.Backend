using Demo.DAL.Models;

namespace Demo.DAL.Dto;

public class WorkerDto
{
    public long TelegramId { get; set; }

    public string FullName { get; set; }
    
    public Department Department { get; set; } 

    public string? TelegramUsername { get; set; }
    
    public WorkerStatus WorkerStatus { get; set; }
}