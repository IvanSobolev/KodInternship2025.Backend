using Demo.DAL.Abstractions;
using Demo.DAL.Models;
using TaskStatus = Demo.DAL.Models.TaskStatus;

namespace Demo.DAL.Repositories.Interfaces;

public interface IWorkerRepository
{
   Task<Result<Worker?>> GetByIdAsync(long id);
   
   // TODO M Просмотр списка работников и фильтрация по статусам и департаменту
   Task<Result<IEnumerable<Worker>>> GetAllAsync(WorkerStatusDto? workerStatusDto = null, Department? department = null);

   Task<Result<Worker>> AddAsync(string fullName, long telegramId, Department department = Department.None, string? telegramUsername = null);
   
   Task<Result<Worker>> UpdateAsync(long telegramId,
      string? fullName = null,
      Department? department = null, 
      string? telegramUsername = null);
   
   Task<Result> DeleteAsync(long id);
}