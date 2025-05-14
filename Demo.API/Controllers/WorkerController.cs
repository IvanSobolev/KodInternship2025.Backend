using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.DAL.Enums;
using Demo.Managers.Implementations;
using Demo.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers;

/// <summary>
/// Контроллер для управления данными работников.
/// </summary>
[ApiController]
[Route("api/workers")] // Изменен базовый маршрут на множественное число
public class WorkerController (IWorkerManager manager) : ControllerBase
{
    private readonly IWorkerManager _manager = manager;

    // <summary>
    /// Получает работника по его Telegram ID.
    /// </summary>
    /// <param name="telegramId">Уникальный Telegram ID работника.</param>
    /// <returns>Найденный работник.</returns>
    /// <response code="200">Работник успешно найден и возвращен.</response>
    /// <response code="404">Работник с указанным Telegram ID не найден.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpGet("{telegramId:long}")] // Используем telegramId как идентификатор ресурса
    [ProducesResponseType(typeof(WorkerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByIdAsync(long telegramId)
    {
        var result = await _manager.GetByIdAsync(telegramId);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Получает список всех работников с возможностью фильтрации.
    /// </summary>
    /// <param name="filters">Параметры фильтрации работников.</param>
    /// <returns>Список работников, соответствующих критериям фильтрации.</returns>
    /// <response code="200">Список работников успешно возвращен.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpGet] // Изменен с POST на GET, фильтры через query string
    [ProducesResponseType(typeof(IEnumerable<WorkerDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllAsync([FromQuery]WorkersFilterDto? filters)
    {
        var result = await _manager.GetAllAsync(filters);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Добавляет нового работника.
    /// </summary>
    /// <param name="newWorkerDto">Данные для добавления нового работника.</param>
    /// <returns>Добавленный работник.</returns>
    /// <response code="201">Работник успешно добавлен. Возвращает созданного работника и ссылку на него.</response>
    /// <response code="400">Некорректные данные для добавления работника (например, дубликат Telegram ID).</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpPost] // Убран избыточный сегмент "add/"
    [ProducesResponseType(typeof(WorkerDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddAsync([FromBody]AddWorkerDto newWorkerDto)
    {
        var result =  await _manager.AddAsync(newWorkerDto);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Обновляет данные существующего работника.
    /// </summary>
    /// <param name="updateWorkerDto">Данные для обновления работника. Telegram ID в теле должен совпадать с ID в пути или может быть опущен.</param>
    /// <returns>Обновленный работник.</returns>
    /// <remarks>
    /// Используйте этот метод для частичного или полного обновления данных работника.
    /// Передавайте только те поля, которые необходимо изменить.
    /// Поля, не переданные в теле запроса (null), не будут изменены.
    /// </remarks>
    /// <response code="200">Данные работника успешно обновлены. Возвращает обновленного работника.</response>
    /// <response code="400">Некорректные данные для обновления или ID в пути и теле не совпадают.</response>
    /// <response code="404">Работник с указанным Telegram ID не найден.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpPut] // ID ресурса в пути, убран избыточный сегмент "add/"
    [ProducesResponseType(typeof(WorkerDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateAsync(UpdateWorkerDto updateWorkerDto)
    {
        var result = await _manager.UpdateAsync(updateWorkerDto);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    /// <summary>
    /// Удаляет работника по его Telegram ID.
    /// </summary>
    /// <param name="telegramId">Telegram ID удаляемого работника.</param>
    /// <response code="204">Работник успешно удален.</response>
    /// <response code="404">Работник с указанным Telegram ID не найден.</response>
    /// <response code="500">Внутренняя ошибка сервера.</response>
    [HttpDelete("{telegramId:long}")] // ID ресурса в пути, убран избыточный сегмент "delete/"
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteAsync(long telegramId)
    {
        var result = await _manager.DeleteAsync(telegramId);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }
}