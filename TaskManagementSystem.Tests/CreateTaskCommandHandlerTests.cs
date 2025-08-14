using Moq;
using TaskManagementSystem.Application.Features.Tasks.Commands.CreateTask;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;
using Xunit;

namespace TaskManagementSystem.Tests;

public class CreateTaskCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_ReturnsCreatedTask()
    {
        // Arrange
        var mockRepo = new Mock<ITaskRepository>();
        mockRepo.Setup(repo => repo.AddAsync(It.IsAny<TaskItem>()))
            .Returns(Task.CompletedTask)
            .Callback<TaskItem>(task => task.Id = 1);
        
        var handler = new CreateTaskCommandHandler(mockRepo.Object);
        var command = new CreateTaskCommand { Description = "Test Task" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Task", result.Description);
        mockRepo.Verify(repo => repo.AddAsync(It.IsAny<TaskItem>()), Times.Once);
    }
}