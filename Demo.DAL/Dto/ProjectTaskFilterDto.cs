using Demo.DAL.Enums;
using TaskStatus = Demo.DAL.Enums.TaskStatus;

namespace Demo.DAL.Dto;

public class ProjectTaskFilterDto
{
    public Department? Department { get; set; }
    public TaskStatus? WorkerStatusDto { get; set; }
}