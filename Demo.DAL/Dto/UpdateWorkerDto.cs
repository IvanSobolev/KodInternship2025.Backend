using Demo.DAL.Enums;
using Demo.DAL.Models;

namespace Demo.DAL.Dto;

public class UpdateWorkerDto (long telegramId, string? fullName = null, Department? department = null, string? telegramUsername = null)
{
    public long TelegramId { get; set; } = telegramId;
    public string? FullName { get; set; } = fullName;
    public Department? Department { get; set; } = department;
    public string? TelegramUsername { get; set; } = telegramUsername;
}