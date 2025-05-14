using Demo.DAL.Enums;
using Demo.DAL.Models;

namespace Demo.DAL.Dto;

public class AddWorkerDto (long telegramId, string fullName, Department department = Department.None, string? telegramUsername = null)
{
    public long TelegramId { get; set; } = telegramId;
    public string FullName { get; set; } = fullName;
    public Department Department { get; set; } = department;
    public string? TelegramUsername { get; set; } = telegramUsername;
}