using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.DAL.Enums;
using Demo.DAL.Models;
using Demo.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Demo.DAL.Repositories.Implementations;

public class PostgresWorkerRepository (DemoDbContext demoDbContext) : IWorkerRepository
{
    private readonly DemoDbContext _dbContext = demoDbContext;
    
    public async Task<Result<WorkerDto?>> GetByIdAsync(long id)
    {
        try
        {
            var worker = await _dbContext.Workers.Select(w => new WorkerDto
            {
                TelegramId = w.TelegramId,
                FullName = w.FullName,
                Department = w.Department,
                TelegramUsername = w.TelegramUsername,
                WorkerStatus = w.WorkerStatus
            }).FirstOrDefaultAsync(w => w.TelegramId == id);

            if (worker == null)
            {
                Result<WorkerDto?>.Failure("User not found", 404);
            }

            return Result<WorkerDto>.Success(worker);
        }
        catch (Exception ex)
        {
            return Result<WorkerDto?>.Failure(ex.Message, 500);
        }
    }

    public async Task<Result<ICollection<long>>> GetTelegramIdInDepartment(Department department)
    {
        var q = _dbContext.Workers.AsQueryable();

        q = q.Where(w => w.Department == department);

        var workers = await q.Select(w => w.TelegramId).ToListAsync();

        return Result<ICollection<long>>.Success(workers);
    }

    public async Task<Result<IEnumerable<WorkerDto>>> GetAllAsync(WorkerStatus? workerStatusDto = null, Department? department = null)
    {
        var q = _dbContext.Workers.AsQueryable();
        if (workerStatusDto.HasValue)
        {
            q = q.Where(w => w.WorkerStatus == workerStatusDto);
        }

        if (department.HasValue)
        {
            q = q.Where(w => w.Department == department);
        }

        var workers = await q.Select(w => new WorkerDto
        {
            TelegramId = w.TelegramId,
            FullName = w.FullName,
            Department = w.Department,
            TelegramUsername = w.TelegramUsername,
            WorkerStatus = w.WorkerStatus
        }).OrderBy(w => w.FullName)
        .ToListAsync();

        return Result<IEnumerable<WorkerDto>>.Success(workers);
    }

    public async Task<Result<WorkerDto>> AddAsync(long telegramId, string fullName,
        Department department = Department.None, string? telegramUsername = null)
    {
        try
        {
            Worker worker = new Worker
            {
                TelegramId = telegramId,
                FullName = fullName,
                Department = department,
                TelegramUsername = telegramUsername,
                WorkerStatus = WorkerStatus.Resting
            };
            await _dbContext.Workers.AddAsync(worker);
            await _dbContext.SaveChangesAsync();

            return Result<WorkerDto>.Success(new WorkerDto
            {
                TelegramId = worker.TelegramId,
                FullName = worker.FullName,
                Department = worker.Department,
                TelegramUsername = worker.TelegramUsername,
                WorkerStatus = worker.WorkerStatus
            });
        }
        catch (Exception ex)
        {
            return Result<WorkerDto>.Failure(ex.Message, 500);
        }
    }

    public async Task<Result<WorkerDto>> UpdateAsync(long telegramId, string? fullName = null, Department? department = null,
        string? telegramUsername = null)
    {
        try
        {
            var query = @"
                UPDATE ""Workers""
                SET 
                    ""FullName"" = COALESCE(@fullName, ""FullName""),
                    ""Department"" = COALESCE(@department, ""Department""),
                    ""TelegramUsername"" = COALESCE(@telegramUsername, ""TelegramUsername"")
                WHERE ""TelegramId"" = @telegramId
                RETURNING *;";

            var parameters = new[]
            {
                new NpgsqlParameter("@fullName", fullName ?? (object)DBNull.Value),
                new NpgsqlParameter("@department", department?.ToString() ?? (object)DBNull.Value),
                new NpgsqlParameter("@telegramUsername", telegramUsername ?? (object)DBNull.Value),
                new NpgsqlParameter("@telegramId", telegramId)
            };

            var updatedWorker = (await _dbContext.Workers
                .FromSqlRaw(query, parameters)
                .AsNoTracking()
                .ToListAsync())
                .FirstOrDefault();
        
            if (updatedWorker == null)
            {
                return Result<WorkerDto>.Failure("Worker not found", 404);
            }

            var workerDto = new WorkerDto
            {
                TelegramId = updatedWorker.TelegramId,
                FullName = updatedWorker.FullName,
                Department = updatedWorker.Department,
                TelegramUsername = updatedWorker.TelegramUsername,
                WorkerStatus = updatedWorker.WorkerStatus
            };
            return Result<WorkerDto>.Success(workerDto);
        }
        catch (Exception ex)
        {
            return Result<WorkerDto>.Failure(ex.Message, 500);
        }
    }

    public async Task<Result> DeleteAsync(long id)
    {
        try
        {
            var rowsAffected = await _dbContext.Database.ExecuteSqlRawAsync(
                @"DELETE FROM ""Workers"" WHERE ""TelegramId"" = {0}", id);

            if (rowsAffected == 0)
            {
                return Result.Failure("Worker not found", 404);
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message, 500);
        }
    }
}