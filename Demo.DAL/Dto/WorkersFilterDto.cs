using Demo.DAL.Enums;

namespace Demo.DAL.Dto;

public class WorkersFilterDto
{
    public WorkerStatus? WorkerStatusDto { get; set; }
    public Department? Department { get; set; }

}