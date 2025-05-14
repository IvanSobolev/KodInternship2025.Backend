using Demo.DAL.Abstractions;
using Demo.DAL.Dto;
using Demo.DAL.Enums;
using Demo.Managers.Implementations;
using Demo.Managers.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkerController (IWorkerManager manager) : ControllerBase
{
    private readonly IWorkerManager _manager = manager;

    [HttpGet]
    [Route("get/byid/{id}")]
    public async Task<IActionResult> GetByIdAsync(long id)
    {
        var result = await _manager.GetByIdAsync(id);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpPost]
    [Route("get/all/")]
    public async Task<IActionResult> GetAllAsync([FromBody]WorkersFilterDto filters)
    {
        var result = await _manager.GetAllAsync(filters);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpPost]
    [Route("add/")]
    public async Task<IActionResult> AddAsync([FromBody]AddWorkerDto newWorker)
    {
        var result =  await _manager.AddAsync(newWorker);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpPut]
    [Route("add/")]
    public async Task<IActionResult> UpdateAsync(UpdateWorkerDto updateField)
    {
        var result = await _manager.UpdateAsync(updateField);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }

    [HttpDelete]
    [Route("delete/{id}")]
    public async Task<IActionResult> DeleteAsync(long id)
    {
        var result = await _manager.DeleteAsync(id);
        if (result.IsSuccess)
        {
            return Ok();
        }

        return Problem(result.Error, statusCode: result.ErrorCode);
    }
}