using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.DAL.Enums;
using Demo.DAL.Models;
using TaskStatus = Demo.DAL.Enums.TaskStatus;

namespace Demo.DAL.Repositories.Interfaces;

public interface IProjectTaskRepository
{
    Task<Result<ProjectTaskDto?>> GetByIdAsync(Guid id);
    
    
    // TODO M Фильтрация списка задач по статусам и департаментам.
    Task<Result<IEnumerable<ProjectTaskDto>>> GetAllAsync(Department? department = null, TaskStatus? status = null);
    // TODO W Проверка доступных заданий по id пользователя
    Task<Result<IEnumerable<ProjectTaskDto>>> GetAllForUserGetAsync(long id);
    
    Task<Result<ProjectTaskDto>> GetTaskByWorkerIdAsync(long id);

    // TODO M Добавление задачи (Отправка уведомления всем в департаменте при добавлении)
    Task<Result<ProjectTaskDto>> AddAsync(string title, string text, Department department);
    
    //TODO M Обновление задачи (ЕСЛИ она в статусе нужно сделать)
    Task<Result<ProjectTaskDto>> UpdateAsync(Guid id, 
        string? title = null, 
        string? text = null, 
        Department? department = null);
    
    // TODO M Удаление задачи в любом статусе (По итогу должно быть уведомление пользователю, у которого эта задача)
    Task<Result> DeleteAsync(Guid id);
    
    
    
    
    // TODO W Принять задачу (проблема с асинхронностью. Сложный запрос с постгрой для исправления)
    Task<Result<ProjectTaskDto>> AcceptTaskAsync(long tgId, Guid id);
    
    // TODO W Отправка таска на проверку.
    Task<Result> CompleteTaskAsync(Guid task);
    
    // TODO M Подтверждение выполнения таска (после проверки задания)
    Task<Result> FinishAsync(Guid id);
    
    // TODO M Не правильное выполнение задачи (после проверки задания)
    Task<Result> CancelAsync(Guid id);
}