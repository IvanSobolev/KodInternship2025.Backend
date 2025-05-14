using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectTaskController (IProjectTaskManager manager) : ControllerBase
{
    private readonly IProjectTaskManager _manager = manager;

    [HttpGet]
    [Route("get/byid/{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _manager.GetByIdAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpPost]
    [Route("get/{id}")]
    public async Task<IActionResult> GetAllAsync([FromBody] ProjectTaskFilterDto filter)
    {
        var result = await _manager.GetAllAsync(filter);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpGet]
    [Route("get/foruser/{workerid}")]
    public async Task<IActionResult> GetAllForUserGetAsync(long workerid)
    {
        var result = await _manager.GetAllForUserGetAsync(workerid);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpGet]
    [Route("get/byuser/{id}")]
    public async Task<IActionResult> GetTaskByWorkerIdAsync(long id)
    {
        var result = await _manager.GetTaskByWorkerIdAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpPost]
    [Route("add")]
    public async Task<IActionResult> AddAsync([FromBody]AddProjectTaskDto newTask)
    {
        var result =  await _manager.AddAsync(newTask);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpPut]
    [Route("update")]
    public async Task<IActionResult> UpdateAsync([FromBody] UpdateTaskDto updateTask)
    {
        var result =  await _manager.UpdateAsync(updateTask);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }
    
    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _manager.DeleteAsync(id);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpPut]
    [Route("accept/{tgId}/{id}")]
    public async Task<IActionResult> AcceptTaskAsync(long tgId, Guid id)
    {
        var result = await _manager.AcceptTaskAsync(tgId, id);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpPut]
    [Route("complite/{task}")]
    public async Task<IActionResult> CompleteTaskAsync(Guid task)
    {
        var result = await _manager.CompleteTaskAsync(task);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpPut]
    [Route("finish/{id}")]
    public async Task<IActionResult> FinishAsync(Guid id)
    {
        var result = await _manager.FinishAsync(id);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpPut]
    [Route("cancel/{id}")]
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