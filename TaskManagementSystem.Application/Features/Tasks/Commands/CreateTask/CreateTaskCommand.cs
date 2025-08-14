// CreateTaskCommand.cs
using MediatR;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommand : IRequest<TaskItem>
{
    public string Description { get; set; } = string.Empty;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
}