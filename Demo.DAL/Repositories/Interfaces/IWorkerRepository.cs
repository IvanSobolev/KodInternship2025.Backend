using Demo.DAL.Abstractions;
using Demo.DAL.Models;
using TaskStatus = System.Threading.Tasks.TaskStatus;

namespace Demo.DAL.Repositories.Interfaces;

public interface IWorkerRepository
{
   Task<Result<Worker?>> GetByIdAsync(Guid id);
   Task<Result<Worker?>> GetByTelegramUserIdAsync(long telegramUserId);
   Task<Result<IEnumerable<Worker>>> GetAllAsync();
   Task<Result<IEnumerable<Worker>>> GetWorkersByDepartmentAsync(Department department);

   Task<Result<Worker>> AddAsync(string fullName, long telegramId, Department department, string? telegramUsername = null);
   Task<Result<Worker>> UpdateAsync(Guid id, 
      string fullName, 
      long telegramId, 
      string telegramUsername,
      Department department);
   
   Task<Result<Worker>> PatchAsync(Guid id, 
      string? fullName = null, 
      long? telegramId = null, 
      Department? department = null, 
      string? telegramUsername = null);
   Task<Result> DeleteAsync(Guid id);
}