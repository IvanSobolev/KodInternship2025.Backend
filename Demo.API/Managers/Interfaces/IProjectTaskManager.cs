using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.DAL.Enums;
using Demo.DAL.Models;
using TaskStatus = Demo.DAL.Enums.TaskStatus;

namespace Demo.Managers.Interfaces;

public interface IProjectTaskManager
{
    Task<Result<ProjectTaskDto?>> GetByIdAsync(Guid id);
    Task<Result<IEnumerable<ProjectTaskDto>>> GetAllAsync(ProjectTaskFilterDto? filter);
    Task<Result<IEnumerable<ProjectTaskDto>>> GetAllForUserGetAsync(long id);
    Task<Result<ProjectTaskDto>> GetTaskByWorkerIdAsync(long id);
    Task<Result<ProjectTaskDto>> AddAsync(AddProjectTaskDto newTask);
    Task<Result<ProjectTaskDto>> UpdateAsync(UpdateTaskDto updateTask);
    Task<Result> DeleteAsync(Guid id);
    Task<Result<ProjectTaskDto>> AcceptTaskAsync(long tgId, Guid id);
    Task<Result> CompleteTaskAsync(Guid task);
    Task<Result> FinishAsync(Guid id);
    Task<Result> CancelAsync(Guid id);
}