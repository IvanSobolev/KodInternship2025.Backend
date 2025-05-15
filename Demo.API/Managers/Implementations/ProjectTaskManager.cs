using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.DAL.Enums;
using Demo.DAL.Repositories.Interfaces;
using Demo.Kafka;
using Demo.Managers.Interfaces;

namespace Demo.Managers.Implementations;

public class ProjectTaskManager(IProjectTaskRepository repository, IKafkaProducerService kafkaProducer, IWorkerRepository workerRepository) : IProjectTaskManager
{
    private readonly IProjectTaskRepository _repository = repository;
    private readonly IKafkaProducerService _kafkaProducer = kafkaProducer;
    private readonly IWorkerRepository _workerRepository = workerRepository;
    
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
         var addResult = await _repository.AddAsync(newTask.Title, newTask.Text, newTask.Department);

        if (!addResult.IsSuccess)
        {
            Console.WriteLine("Ошибка при добавлении задачи в БД: {Error}", addResult.Error);
            return addResult;
        }

        var createdTaskDto = addResult.Value;
        List<long> recipientTelegramIds = new List<long>();
        var workersResult = await _workerRepository.GetTelegramIdInDepartment(createdTaskDto.Department);
        try
        {
            if (workersResult.IsSuccess)
            {
                recipientTelegramIds.AddRange(workersResult.Value);
            }
            else
            {
                Console.WriteLine("Не удалось получить список работников для уведомления о задаче {TaskId}: {Error}",
                    createdTaskDto.Id, workersResult.Error);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при получении списка работников для уведомления о задаче {TaskId}", createdTaskDto.Id);
        }

        
        if (recipientTelegramIds.Any())
        {
            var kafkaMessage = new NewTaskMessage
            {
                TaskId = createdTaskDto.Id,
                Title = createdTaskDto.Title,
                Text = createdTaskDto.Text,
                Department = createdTaskDto.Department.ToString(),
                RecipientTelegramIds = recipientTelegramIds
            };

            try
            {
                await _kafkaProducer.ProduceNewTaskMessageAsync(kafkaMessage);
                Console.WriteLine("Сообщение о новой задаче {TaskId} успешно отправлено в Kafka.", createdTaskDto.Id);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке сообщения о новой задаче {TaskId} в Kafka.");
            }
        }
        else
        {
           Console.WriteLine("Нет получателей для уведомления о новой задаче {TaskId}. Сообщение в Kafka не отправлялось.");
        }

        return Result<ProjectTaskDto>.Success(createdTaskDto);
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
        var taskResult = await _repository.GetByIdAsync(id);
        if (!taskResult.IsSuccess || taskResult.Value == null)
        {
            Console.WriteLine("Задача {TaskId} не найдена при попытке её завершения.");
            return Result.Failure(taskResult.Error ?? "Задача не найдена.", taskResult.ErrorCode ?? 404);
        }
        var taskDto = taskResult.Value;

        // Выполняем операцию завершения задачи в репозитории
        var finishResult = await _repository.FinishAsync(id);

        if (finishResult.IsSuccess)
        {
            // Если задача успешно завершена, отправляем уведомление
            if (taskDto.AssignedWorkerId.HasValue)
            {
                var notificationMessage = new SimpleNotificationMessage(
                    content: $"Задача \"{taskDto.Title}\" (ID: {taskDto.Id}) была успешно завершена и принята.",
                    messageType: "TaskFinishedSuccess",
                    targetUserTelegramIds: new List<long> { taskDto.AssignedWorkerId.Value }
                );
                try
                {
                    await _kafkaProducer.ProduceSimpleNotificationAsync(notificationMessage);
                    Console.WriteLine("Уведомление об успешном завершении задачи {TaskId} отправлено работнику {WorkerId}.",
                        taskDto.Id, taskDto.AssignedWorkerId.Value);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при отправке Kafka-уведомления об успешном завершении задачи {TaskId} работнику {WorkerId}.");
                    // Не возвращаем ошибку клиенту API, так как основная операция (завершение задачи) прошла успешно.
                }
            }
            else
            {
               Console.WriteLine("Задача {TaskId} успешно завершена, но не была назначена работнику. Уведомление не отправлено.");
            }
        }
        else
        {
            Console.WriteLine("Ошибка при завершении задачи {TaskId} в репозитории: {Error}");
        }
        return finishResult;
    }

    public async Task<Result> CancelAsync(Guid id)
    {
        var taskResult = await _repository.GetByIdAsync(id);
        if (!taskResult.IsSuccess || taskResult.Value == null)
        {
            Console.WriteLine("Задача {TaskId} не найдена при попытке отменить ее ревью.");
            return Result.Failure(taskResult.Error ?? "Задача не найдена.", taskResult.ErrorCode ?? 404);
        }
        var taskDto = taskResult.Value;

        // Выполняем основное действие - отмена ревью
        var cancelResult = await _repository.CancelAsync(id);

        if (cancelResult.IsSuccess)
        {
            Console.WriteLine("Ревью задачи {TaskId} отменено, задача возвращена в работу.");
            if (taskDto.AssignedWorkerId.HasValue)
            {
                var notificationContent = $"Задача \"{taskDto.Title}\" (ID: {taskDto.Id}) была возвращена в работу после ревью. Требуются доработки.";
                var kafkaMessage = new SimpleNotificationMessage(
                    content: notificationContent,
                    messageType: "TaskReviewCancelled",
                    targetUserTelegramIds: new List<long> { taskDto.AssignedWorkerId.Value }
                );

                try
                {
                    await _kafkaProducer.ProduceSimpleNotificationAsync(kafkaMessage);
                    Console.WriteLine("Уведомление об отмене ревью задачи {TaskId} отправлено работнику {WorkerId}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ошибка при отправке Kafka-уведомления об отмене ревью задачи {TaskId}.");
                }
            }
            else
            {
                Console.WriteLine("Ревью задачи {TaskId} отменено, но задача не была никому назначена. Уведомление не отправлено.");
            }
        }
        else
        {
            Console.WriteLine("Ошибка при отмене ревью задачи {TaskId}: {Error}. Код ошибки: {ErrorCode}");
        }
        return cancelResult;
    }
}