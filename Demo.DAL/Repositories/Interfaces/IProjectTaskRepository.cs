using Demo.DAL.Abstractions;
using Demo.DAL.Models;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace Demo.DAL.Repositories.Interfaces;

public interface IProjectTaskRepository
{
    Task<Result<ProjectTask?>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<ProjectTask>>> GetAllAsync(Department? department = null, TaskStatus? status = null);
    Task<Result<ProjectTask>> GetTaskByWorkerIdAsync(Guid workerId);
    Task<Result<ProjectTask>> GetTaskByWorkerTelegramUserIdAsync(long telegramUserId);

    Task<Result<ProjectTask>> AddAsync(string title, string text, TaskStatus status, Department department);
    Task<Result<ProjectTask>> UpdateAsync(Guid id, 
        string title, 
        string text, 
        TaskStatus status, 
        Department department,
        Guid? assignedWorkerId);
    
    Task<Result<ProjectTask>> PatchAsync(Guid id, 
        string? title = null, 
        string? text = null, 
        TaskStatus? status = null, 
        Department? department = null,
        Guid? assignedWorkerId = null);
    Task<Result> DeleteAsync(Guid id);
}