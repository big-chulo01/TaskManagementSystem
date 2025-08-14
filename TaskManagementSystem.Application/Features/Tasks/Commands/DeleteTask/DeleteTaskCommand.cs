using MediatR;

namespace TaskManagementSystem.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskCommand : IRequest<Unit>
{
    public int Id { get; set; }
}