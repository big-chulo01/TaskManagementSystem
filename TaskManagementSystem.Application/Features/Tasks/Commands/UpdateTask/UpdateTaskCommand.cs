using MediatR;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskCommand : IRequest<TaskItem>
{
    public TaskItem TaskItem { get; set; } = null!;
}