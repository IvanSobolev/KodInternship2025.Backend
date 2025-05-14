using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.DAL.Enums;
using Demo.DAL.Repositories.Interfaces;
using Demo.Managers.Interfaces;

namespace Demo.Managers.Implementations;

public class ProjectTaskManager(IProjectTaskRepository repository) : IProjectTaskManager
{
    private readonly IProjectTaskRepository _repository = repository;
    
    public async Task<Result<ProjectTaskDto?>> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Result<IEnumerable<ProjectTaskDto>>> GetAllAsync(ProjectTaskFilterDto? filter)
    {
        return await _repository.GetAllAsync(filter?.Department, filter?.WorkerStatusDto);
    }

    public async Task<Result<IEnumerable<ProjectTaskDto>>> GetAllForUserGetAsync(long id)
    {
        return await _repository.GetAllForUserGetAsync(id);
    }

    public async Task<Result<ProjectTaskDto>> GetTaskByWorkerIdAsync(long id)
    {
        return await _repository.GetTaskByWorkerIdAsync(id);
    }

    public async Task<Result<ProjectTaskDto>> AddAsync(AddProjectTaskDto newTask)
    {
        return await _repository.AddAsync(newTask.Title, newTask.Text, newTask.Department);
    }

    public async Task<Result<ProjectTaskDto>> UpdateAsync(UpdateTaskDto updateTask)
    {
        return await _repository.UpdateAsync(updateTask.Id,
            updateTask.Title,
            updateTask.Text,
            updateTask.Department);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<Result<ProjectTaskDto>> AcceptTaskAsync(long tgId, Guid id)
    {
        return await _repository.AcceptTaskAsync(tgId, id);
    }

    public async Task<Result> CompleteTaskAsync(Guid task)
    {
        return await _repository.CompleteTaskAsync(task);
    }

    public async Task<Result> FinishAsync(Guid id)
    {
        return await _repository.FinishAsync(id);
    }

    public async Task<Result> CancelAsync(Guid id)
    {
        return await _repository.CancelAsync(id);
    }
}