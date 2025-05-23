﻿using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.DAL.Enums;
using Demo.DAL.Models;
using TaskStatus = Demo.DAL.Enums.TaskStatus;

namespace Demo.DAL.Repositories.Interfaces;

public interface IWorkerRepository
{
   Task<Result<WorkerDto?>> GetByIdAsync(long id);

   Task<Result<ICollection<long>>> GetTelegramIdInDepartment(Department department);
   
   // TODO M Просмотр списка работников и фильтрация по статусам и департаменту
   Task<Result<IEnumerable<WorkerDto>>> GetAllAsync(WorkerStatus? workerStatusDto = null, Department? department = null);

   Task<Result<WorkerDto>> AddAsync(long telegramId, string fullName, Department department = Department.None, string? telegramUsername = null);
   
   Task<Result<WorkerDto>> UpdateAsync(long telegramId,
      string? fullName = null,
      Department? department = null, 
      string? telegramUsername = null);
   
   Task<Result> DeleteAsync(long id);
}