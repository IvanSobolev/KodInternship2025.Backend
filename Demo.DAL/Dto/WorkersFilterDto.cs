using Demo.DAL.Enums;
using Demo.DAL.Models;

namespace Demo.DAL.Dto;

public class WorkersFilterDto (WorkerStatus? workerStatus = null, Department? department = null)
{
    public WorkerStatus? WorkerStatusDto { get; set; } = workerStatus;
    public Department? Department { get; set; } = department;

}