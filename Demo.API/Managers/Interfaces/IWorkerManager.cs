using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.DAL.Models;

namespace Demo.Managers.Interfaces;

public interface IWorkerManager
{
    Task<Result<WorkerDto?>> GetByIdAsync(long id);
    Task<Result<IEnumerable<WorkerDto>>> GetAllAsync(WorkersFilterDto? filters);
    Task<Result<WorkerDto>> AddAsync(AddWorkerDto newWorker);
    Task<Result<WorkerDto>> UpdateAsync(UpdateWorkerDto updateField);
    Task<Result> DeleteAsync(long id);
}