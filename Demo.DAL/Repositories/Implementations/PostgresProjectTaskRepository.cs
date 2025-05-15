using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.DAL.Enums;
using Demo.DAL.Models;
using Demo.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using TaskStatus = Demo.DAL.Enums.TaskStatus;

namespace Demo.DAL.Repositories.Implementations;

public class PostgresProjectTaskRepository (DemoDbContext dbContext) : IProjectTaskRepository
{
    private readonly DemoDbContext _dbContext = dbContext;
    
    private static ProjectTaskDto MapToDto(ProjectTask task)
    {
        if (task == null) return null;
        return new ProjectTaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Text = task.Text,
            Status = task.Status,
            Department = task.Department,
            AssignedWorkerId = task.AssignedWorkerId,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
    
    public async Task<Result<ProjectTaskDto?>> GetByIdAsync(Guid id)
    {
        try
        {
            var projectTask = await _dbContext.Tasks.Select(t => new ProjectTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Text = t.Text,
                Status = t.Status,
                AssignedWorkerId = t.AssignedWorkerId,
                Department = t.Department,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).FirstOrDefaultAsync(t => t.Id == id);

            if (projectTask == null)
            {
                return Result<ProjectTaskDto?>.Failure("Task not found", 404);
            }

            return Result<ProjectTaskDto?>.Success(projectTask);
        }
        catch(Exception ex)
        {
            return Result<ProjectTaskDto?>.Failure(ex.Message, 500);
        }
    }

    public async Task<Result<IEnumerable<ProjectTaskDto>>> GetAllAsync(Department? department = null, TaskStatus? status = null)
    {
        try
        {
            var query = _dbContext.Tasks.AsQueryable().Select(t => new ProjectTaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Text = t.Text,
                Status = t.Status,
                AssignedWorkerId = t.AssignedWorkerId,
                Department = t.Department,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            });

            if (department.HasValue)
            {
                query = query.Where(t => t.Department == department.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == status.Value);
            }

            var tasks = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
            return Result<IEnumerable<ProjectTaskDto>>.Success(tasks);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ProjectTaskDto>>.Failure(ex.Message, 500);
        }
    }

    public async Task<Result<IEnumerable<ProjectTaskDto>>> GetAllForUserGetAsync(long id)
    {
        try
        {
            var worker = await _dbContext.Workers
                .FirstOrDefaultAsync(w => w.TelegramId == id);

            if (worker == null)
            {
                return Result<IEnumerable<ProjectTaskDto>>.Failure("Worker not found", 404);
            }
            
            var tasks = await _dbContext.Tasks
                .Where(pt => pt.Department == worker.Department && pt.Status == TaskStatus.ToDo)
                .Select(t => new ProjectTaskDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    Text = t.Text,
                    Status = t.Status,
                    AssignedWorkerId = t.AssignedWorkerId,
                    Department = t.Department,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
            return Result<IEnumerable<ProjectTaskDto>>.Success(tasks);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<ProjectTaskDto>>.Failure(ex.Message, 500);
        }
    }

    public async Task<Result<ProjectTaskDto>> GetTaskByWorkerIdAsync(long id)
    {
        try
        {
            var task = await _dbContext.Tasks
                .FirstOrDefaultAsync(pt => pt.AssignedWorkerId == id);

            if (task == null)
            {
                return Result<ProjectTaskDto>.Failure("No active task found for this worker.", 404);
            }
            return Result<ProjectTaskDto>.Success(MapToDto(task));
        }
        catch (Exception ex)
        {
            return Result<ProjectTaskDto>.Failure(ex.Message, 500);
        }
    }

    public async Task<Result<ProjectTaskDto>> AddAsync(string title, string text, Department department)
    {
        try
        {
            var newTask = new ProjectTask
            {
                Title = title,
                Text = text,
                Department = department,
                Status = TaskStatus.ToDo,
            };

            await _dbContext.Tasks.AddAsync(newTask);
            await _dbContext.SaveChangesAsync();
            return Result<ProjectTaskDto>.Success(MapToDto(newTask));
        }
        catch (Exception ex)
        {
            return Result<ProjectTaskDto>.Failure(ex.Message, 500);
        }
    }

    public async Task<Result<ProjectTaskDto>> UpdateAsync(Guid id, string? title = null, string? text = null, Department? department = null)
    {
        try
        {
            var task = await _dbContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
            {
                return Result<ProjectTaskDto>.Failure("Task not found.", 404);
            }

            if (task.Status != TaskStatus.ToDo)
            {
                return Result<ProjectTaskDto>.Failure("Task can only be updated if its status is 'ToDo'.", 400);
            }

            bool updated = false;
            if (!string.IsNullOrWhiteSpace(title))
            {
                task.Title = title;
                updated = true;
            }
            if (!string.IsNullOrWhiteSpace(text))
            {
                task.Text = text;
                updated = true;
            }
            if (department.HasValue)
            {
                task.Department = department.Value;
                updated = true;
            }

            if (updated)
            {
                task.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
            }
                
            return Result<ProjectTaskDto>.Success(MapToDto(task));
        }
        catch (Exception ex)
        {
            return Result<ProjectTaskDto>.Failure(ex.Message, 500);
        }
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        try
        {
            var task = await _dbContext.Tasks.Include(t => t.AssignedWorker).FirstOrDefaultAsync(t => t.Id == id);
            if (task == null)
            {
                return Result.Failure("Task not found.", 404);
            }

            _dbContext.Tasks.Remove(task);
            if (task.AssignedWorker != null) task.AssignedWorker.WorkerStatus = WorkerStatus.Resting;
            await _dbContext.SaveChangesAsync();

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message, 500);
        }
    }

    public async Task<Result<ProjectTaskDto>> AcceptTaskAsync(long tgId, Guid id)
    {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadCommitted);

        try
        {
            var worker = await _dbContext.Workers
                                         .FirstOrDefaultAsync(w => w.TelegramId == tgId);
            if (worker == null)
            {
                await transaction.RollbackAsync();
                return Result<ProjectTaskDto>.Failure("Работник не найден.", 404);
            }
            if (worker.WorkerStatus != WorkerStatus.Resting)
            {
                await transaction.RollbackAsync();
                return Result<ProjectTaskDto>.Failure("Работник занят (статус должен быть 'Resting').", 400);
            }
            bool workerHasActiveTask = await _dbContext.Tasks.AnyAsync(pt => pt.AssignedWorkerId == tgId && pt.Status != TaskStatus.Completed);
            if (workerHasActiveTask)
            {
                 await transaction.RollbackAsync();
                 return Result<ProjectTaskDto>.Failure("У работника уже есть активная задача.", 400);
            }


            var now = DateTime.UtcNow;
            ProjectTask? acceptedTask = null;
            
            string statusInProgressString = TaskStatus.InProgress.ToString();
            string statusToDoString = TaskStatus.ToDo.ToString();
            
            FormattableString query = $@"
                UPDATE ""Tasks""
                SET ""Status"" = {statusInProgressString},
                    ""AssignedWorkerId"" = {tgId},
                    ""UpdatedAt"" = {now}
                WHERE ""Id"" = {id}
                  AND ""Status"" = {statusToDoString}
                  AND ""AssignedWorkerId"" IS NULL
                RETURNING ""Id"", ""Title"", ""Text"", ""Status"", ""Department"", ""AssignedWorkerId"", ""CreatedAt"", ""UpdatedAt"";";
            
            List<ProjectTask> returnedTasks = await _dbContext.Tasks
                .FromSqlInterpolated<ProjectTask>(query)
                .ToListAsync();

            acceptedTask = returnedTasks.FirstOrDefault();


            if (acceptedTask == null)
            {
                await transaction.RollbackAsync();
                var taskStillExistsAndIsToDo = await _dbContext.Tasks
                    .AnyAsync(t => t.Id == id && t.Status == TaskStatus.ToDo && t.AssignedWorkerId == null);

                if (!taskStillExistsAndIsToDo)
                {
                    var taskExistsAtAll = await _dbContext.Tasks.AnyAsync(t => t.Id == id);
                    if (!taskExistsAtAll)
                    {
                        return Result<ProjectTaskDto>.Failure("Задача не найдена.", 404);
                    }
                    return Result<ProjectTaskDto>.Failure("Не удалось принять задачу. Возможно, она уже назначена другому работнику или более недоступна.", 409); // 409 Conflict
                }
                return Result<ProjectTaskDto>.Failure("Внутренняя ошибка сервера при попытке принять задачу.", 500);
            }

            worker.WorkerStatus = WorkerStatus.Working;
            worker.AssignedTask = acceptedTask;

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return Result<ProjectTaskDto>.Success(MapToDto(acceptedTask));
        }
        catch (DbUpdateException ex)
        {
            await transaction.RollbackAsync();
            return Result<ProjectTaskDto>.Failure($"Ошибка базы данных при принятии задачи: {ex.InnerException?.Message ?? ex.Message}", 500);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return Result<ProjectTaskDto>.Failure($"Произошла непредвиденная ошибка: {ex.Message}", 500);
        }
    }

    public async Task<Result> CompleteTaskAsync(Guid taskId)
    {
        try
        {
            var task = await _dbContext.Tasks
                .Include(t => t.AssignedWorker)
                .FirstOrDefaultAsync(t => t.Id == taskId);
            
            if (task == null)
            {
                return Result.Failure("Task not found.", 404);
            }

            if (task.Status != TaskStatus.InProgress)
            {
                return Result.Failure("Task must be 'In Progress' to be completed.", 400);
            }

            task.Status = TaskStatus.PendingReview;
            task.UpdatedAt = DateTime.UtcNow;
            if (task.AssignedWorker == null)
            {
                return Result.Failure("Worker is not assigned", 402);
            }
            task.AssignedWorker.WorkerStatus = WorkerStatus.Pending;
            await _dbContext.SaveChangesAsync();
                
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message, 500);
        }
    }

    public async Task<Result> FinishAsync(Guid id)
    {
        try
        {
            var task = await _dbContext.Tasks
                .Include(t => t.AssignedWorker)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return Result.Failure("Task not found.", 404);
            }

            if (task.Status != TaskStatus.PendingReview)
            {
                return Result.Failure("Task must be 'Pending Review' to be finished.", 400);
            }

            task.Status = TaskStatus.Completed;
            task.UpdatedAt = DateTime.UtcNow;

            if (task.AssignedWorker != null)
            {
                task.AssignedWorker.WorkerStatus = WorkerStatus.Resting;
                task.AssignedWorker.AssignedTask = null;
            }

            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message, 500);
        }
    }

    public async Task<Result> CancelAsync(Guid id)
    {
        try
        {
            var task = await _dbContext.Tasks
                .Include(t => t.AssignedWorker)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return Result.Failure("Task not found.", 404);
            }

            if (task.Status != TaskStatus.PendingReview)
            {
                return Result.Failure("Task must be 'Pending Review' to be cancelled (rejected).", 400);
            }

            task.Status = TaskStatus.InProgress; 
                
            if (task.AssignedWorker != null)
            {
                task.AssignedWorker.WorkerStatus = WorkerStatus.Working;
            }

            await _dbContext.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message, 500);
        }
    }
}