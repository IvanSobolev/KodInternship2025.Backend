using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.DAL.Repositories.Interfaces;
using Demo.Managers.Interfaces;

namespace Demo.Managers.Implementations;

public class WorkerManager (IWorkerRepository workerRepository) : IWorkerManager
{
    private readonly IWorkerRepository _workerRepository = workerRepository;
    
    public async Task<Result<WorkerDto?>> GetByIdAsync(long id)
    {
        return await _workerRepository.GetByIdAsync(id);
    }

    public async Task<Result<IEnumerable<WorkerDto>>> GetAllAsync(WorkersFilterDto? filters)
    {
        return await _workerRepository.GetAllAsync(filters?.WorkerStatusDto, filters?.Department);
    }

    public async Task<Result<WorkerDto>> AddAsync(AddWorkerDto newWorker)
    {
        return await _workerRepository.AddAsync(newWorker.TelegramId, newWorker.FullName, newWorker.Department,
            newWorker.TelegramUsername);
    }

    public async Task<Result<WorkerDto>> UpdateAsync(UpdateWorkerDto updateField)
    {
        return await _workerRepository.UpdateAsync(updateField.TelegramId,
            updateField.FullName,
            updateField.Department,
            updateField.TelegramUsername);
    }

    public async Task<Result> DeleteAsync(long id)
    {
        return await _workerRepository.DeleteAsync(id);
    }
}