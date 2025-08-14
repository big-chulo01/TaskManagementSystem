using MediatR;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Application.Features.Tasks.Queries.GetAllTasks;

public class GetAllTasksQuery : IRequest<IEnumerable<TaskItem>> { }