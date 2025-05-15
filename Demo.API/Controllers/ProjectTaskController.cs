using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers;

/// <summary>
/// Контроллер для управления задачами проектов.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ProjectTaskController (IProjectTaskManager manager) : ControllerBase
{
    private readonly IProjectTaskManager _manager = manager;

    /// <summary>
    /// Получает задачу проекта по её уникальному идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор задачи проекта.</param>
    /// <returns>Найденная задача проекта.</returns>
    /// <response code="200">Задача успешно найдена и возвращена.</response>
    /// <response code="404">Задача с указанным ID не найдена.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _manager.GetByIdAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Получает список всех задач проекта с возможностью фильтрации.
    /// </summary>
    /// <param name="filter">Параметры фильтрации задач.</param>
    /// <returns>Список задач проекта, соответствующих критериям фильтрации.</returns>
    /// <response code="200">Список задач успешно возвращен.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectTaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync([FromQuery] ProjectTaskFilterDto? filter)
    {
        var result = await _manager.GetAllAsync(filter);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Получает все задачи, которые может выполнять работник.
    /// </summary>
    /// <param name="workerId">Telegram ID работника.</param>
    /// <returns>Список задач, назначенных пользователю.</returns>
    /// <response code="200">Список задач успешно возвращен.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpGet("avaible-to-user/{workerId:long}")] // Уточнили тип параметра
    [ProducesResponseType(typeof(IEnumerable<ProjectTaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllForUserGetAsync(long workerId)
    {
        var result = await _manager.GetAllForUserGetAsync(workerId);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Получает текущую активную (незавершенную) задачу для указанного пользователя (работника).
    /// </summary>
    /// <param name="workerId">Telegram ID работника.</param>
    /// <returns>Активная задача пользователя или сообщение об отсутствии таковой.</returns>
    /// <response code="200">Активная задача успешно найдена и возвращена.</response>
    /// <response code="404">Активная задача для данного работника не найдена.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpGet("active-for-user/{workerId:long}")]
    [ProducesResponseType(typeof(ProjectTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTaskByWorkerIdAsync(long workerId)
    {
        var result = await _manager.GetTaskByWorkerIdAsync(workerId);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Создает новую задачу проекта.
    /// </summary>
    /// <param name="newTaskDto">Данные для создания новой задачи.</param>
    /// <returns>Созданная задача проекта.</returns>
    /// <response code="201">Задача успешно создана. Возвращает созданную задачу и ссылку на неё.</response>
    /// <response code="400">Некорректные данные для создания задачи.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectTaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)] // Если используется валидация моделей
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddAsync([FromBody]AddProjectTaskDto newTaskDto)
    {
        var result =  await _manager.AddAsync(newTaskDto);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Обновляет существующую задачу проекта.
    /// </summary>
    /// <param name="updateTaskDto">Данные для обновления задачи.</param>
    /// <returns>Обновленная задача проекта.</returns>
    /// <remarks>
    /// Используйте этот метод для частичного или полного обновления задачи.
    /// Передавайте только те поля, которые необходимо изменить.
    /// </remarks>
    /// <response code="200">Задача успешно обновлена. Возвращает обновленную задачу.</response>
    /// <response code="400">Некорректные данные для обновления или задача не может быть обновлена (например, неверный статус).</response>
    /// <response code="404">Задача с указанным ID не найдена.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpPut] // Или HttpPatch, если это частичное обновление
    [ProducesResponseType(typeof(ProjectTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateTaskDto updateTaskDto)
    {
        Console.WriteLine(updateTaskDto.Id);
        var result =  await _manager.UpdateAsync(updateTaskDto);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }
    
    /// <summary>
    /// Удаляет задачу проекта по её уникальному идентификатору.
    /// </summary>
    /// <param name="id">Уникальный идентификатор удаляемой задачи.</param>
    /// <response code="204">Задача успешно удалена.</response>
    /// <response code="404">Задача с указанным ID не найдена.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _manager.DeleteAsync(id);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Назначает задачу указанному работнику (принятие задачи).
    /// </summary>
    /// <param name="tgId">Данные для принятия задачи, содержащие Telegram ID работника.</param>
    /// <param name="taskId">Уникальный идентификатор задачи, которую нужно принять.</param>
    /// <returns>Обновленная задача проекта с назначенным работником.</returns>
    /// <response code="200">Задача успешно принята и назначена работнику.</response>
    /// <response code="400">Некорректные данные или работник/задача не могут быть обработаны (например, работник занят, задача уже назначена).</response>
    /// <response code="404">Задача или работник не найдены.</response>
    /// <response code="409">Конфликт. Не удалось принять задачу (например, уже назначена другому).</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpPost("{taskId:guid}/accept")]
    [ProducesResponseType(typeof(ProjectTaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AcceptTaskAsync(long tgId, Guid taskId)
    {
        var result = await _manager.AcceptTaskAsync(tgId, taskId);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Помечает задачу как выполненную работником (ожидает ревью).
    /// </summary>
    /// <param name="id">Уникальный идентификатор задачи.</param>
    /// <response code="200">Статус задачи успешно изменен на 'Ожидает ревью'.</response>
    /// <response code="400">Задача не может быть помечена как выполненная (например, неверный текущий статус).</response>
    /// <response code="404">Задача не найдена.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpPost("{id:guid}/complete")]
    [ProducesResponseType(StatusCodes.Status200OK)] // Можно вернуть обновленную задачу, если это полезно
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CompleteTaskAsync(Guid id)
    {
        var result = await _manager.CompleteTaskAsync(id);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Окончательно завершает задачу (например, после успешного ревью).
    /// </summary>
    /// <param name="id">Уникальный идентификатор задачи.</param>
    /// <response code="200">Задача успешно завершена.</response>
    /// <response code="400">Задача не может быть завершена (например, неверный текущий статус).</response>
    /// <response code="404">Задача не найдена.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpPost("{id:guid}/finish")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> FinishAsync(Guid id)
    {
        var result = await _manager.FinishAsync(id);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Отменяет ревью задачи, возвращая её в статус "в работе".
    /// </summary>
    /// <param name="id">Уникальный идентификатор задачи.</param>
    /// <response code="200">Ревью задачи успешно отменено.</response>
    /// <response code="400">Ревью задачи не может быть отменено (например, неверный текущий статус).</response>
    /// <response code="404">Задача не найдена.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpPost("{id:guid}/cancel-review")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CancelAsync(Guid id)
    {
        var result = await _manager.CancelAsync(id);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }
}