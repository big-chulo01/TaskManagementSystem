using Moq;
using TaskManagementSystem.Application.Features.Tasks.Queries.GetAllTasks;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;
using Xunit;

namespace TaskManagementSystem.Tests;

public class GetAllTasksQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsAllTasks()
    {
        // Arrange
        var testTasks = new List<TaskItem>
        {
            new() { Id = 1, Description = "Task 1" },
            new() { Id = 2, Description = "Task 2" }
        };

        var mockRepo = new Mock<ITaskRepository>();
        mockRepo.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(testTasks);
        
        var handler = new GetAllTasksQueryHandler(mockRepo.Object);
        var query = new GetAllTasksQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(2, result.Count());
        mockRepo.Verify(repo => repo.GetAllAsync(), Times.Once);
    }
}