using Demo.DAL.Enums;
using TaskStatus = Demo.DAL.Enums.TaskStatus;

namespace Demo.DAL.Dto;

public class ProjectTaskFilterDto(Department? department = null, TaskStatus? status = null)
{
    public Department? Department { get; set; } = department;
    public TaskStatus? Status { get; set; } = status;
}