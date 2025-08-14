using MediatR;
using Microsoft.FeatureManagement;
using Exporter.Abstractions;
using TaskManagementSystem.Application.Features.Tasks.Commands.CreateTask;
using TaskManagementSystem.Application.Features.Tasks.Commands.DeleteTask;
using TaskManagementSystem.Application.Features.Tasks.Commands.UpdateTask;
using TaskManagementSystem.Application.Features.Tasks.Queries.GetAllTasks;
using TaskManagementSystem.Application.Features.Tasks.Queries.GetTaskById;
using TaskManagementSystem.Domain.Entities;

namespace TaskManagementSystem.Presentation;

public class App
{
    private readonly IMediator _mediator;
    private readonly IFeatureManager _featureManager;
    private readonly IEnumerable<IExporter> _exporters;

    public App(IMediator mediator, IFeatureManager featureManager, IEnumerable<IExporter> exporters)
    {
        _mediator = mediator;
        _featureManager = featureManager;
        _exporters = exporters;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("Task Management System");
        Console.WriteLine("----------------------");

        while (true)
        {
            Console.WriteLine("\nMain Menu:");
            Console.WriteLine("1. View All Tasks");
            Console.WriteLine("2. View Task Details");
            Console.WriteLine("3. Add New Task");
            Console.WriteLine("4. Mark Task as Complete");
            Console.WriteLine("5. Delete Task");
            Console.WriteLine("6. Export Tasks");
            Console.WriteLine("0. Exit");

            Console.Write("\nEnter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await ViewAllTasks();
                    break;
                case "2":
                    await ViewTaskDetails();
                    break;
                case "3":
                    await AddNewTask();
                    break;
                case "4":
                    await MarkTaskComplete();
                    break;
                case "5":
                    await DeleteTask();
                    break;
                case "6":
                    await ExportTasks();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    private async Task ViewAllTasks()
    {
        var tasks = await _mediator.Send(new GetAllTasksQuery());
        
        Console.WriteLine("\nAll Tasks:");
        Console.WriteLine("ID\tDescription\t\tStatus\tPriority");
        foreach (var task in tasks)
        {
            Console.WriteLine($"{task.Id}\t{task.Description}\t\t{(task.IsCompleted ? "Completed" : "Pending")}\t{task.Priority}");
        }
    }

    private async Task ViewTaskDetails()
    {
        Console.Write("\nEnter Task ID: ");
        if (!int.TryParse(Console.ReadLine(), out var taskId))
        {
            Console.WriteLine("Invalid Task ID.");
            return;
        }

        var task = await _mediator.Send(new GetTaskByIdQuery { Id = taskId });
        if (task == null)
        {
            Console.WriteLine("Task not found.");
            return;
        }

        Console.WriteLine("\nTask Details:");
        Console.WriteLine($"ID: {task.Id}");
        Console.WriteLine($"Description: {task.Description}");
        Console.WriteLine($"Status: {(task.IsCompleted ? "Completed" : "Pending")}");
        Console.WriteLine($"Priority: {task.Priority}");
        Console.WriteLine($"Created At: {task.CreatedAt:yyyy-MM-dd}");
    }

    private async Task AddNewTask()
    {
        Console.WriteLine("\nAdd New Task");
        
        Console.Write("Description: ");
        var description = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Description cannot be empty.");
            return;
        }

        var command = new CreateTaskCommand 
        { 
            Description = description 
        };

        if (await _featureManager.IsEnabledAsync("TaskPrioritization"))
        {
            Console.WriteLine("Select priority (1-Low, 2-Medium, 3-High): ");
            if (int.TryParse(Console.ReadLine(), out var priority) && priority >= 1 && priority <= 3)
            {
                command.Priority = (TaskPriority)(priority - 1);
            }
        }

        var createdTask = await _mediator.Send(command);
        Console.WriteLine($"Task created successfully with ID: {createdTask.Id}");
    }

    private async Task MarkTaskComplete()
    {
        Console.Write("\nEnter Task ID to mark as complete: ");
        if (!int.TryParse(Console.ReadLine(), out var taskId))
        {
            Console.WriteLine("Invalid Task ID.");
            return;
        }

        // Get existing task first
        var task = await _mediator.Send(new GetTaskByIdQuery { Id = taskId });
        if (task == null)
        {
            Console.WriteLine("Task not found.");
            return;
        }

        // Update the task
        task.IsCompleted = true;
        await _mediator.Send(new UpdateTaskCommand { TaskItem = task });
        Console.WriteLine("Task marked as complete successfully.");
    }

    private async Task DeleteTask()
    {
        Console.Write("\nEnter Task ID to delete: ");
        if (!int.TryParse(Console.ReadLine(), out var taskId))
        {
            Console.WriteLine("Invalid Task ID.");
            return;
        }

        await _mediator.Send(new DeleteTaskCommand { Id = taskId });
        Console.WriteLine("Task deleted successfully.");
    }

    private async Task ExportTasks()
    {
        if (!await _featureManager.IsEnabledAsync("Export"))
        {
            Console.WriteLine("Export feature is not enabled.");
            return;
        }

        Console.WriteLine("\nAvailable Export Formats:");
        var exporters = _exporters.ToList();
        for (int i = 0; i < exporters.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {exporters[i].GetFormatName()}");
        }

        Console.Write("Select export format: ");
        if (!int.TryParse(Console.ReadLine(), out var formatChoice) || formatChoice < 1 || formatChoice > exporters.Count)
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        var selectedExporter = exporters[formatChoice - 1];
        var tasks = await _mediator.Send(new GetAllTasksQuery());
        var exportResult = selectedExporter.Export(tasks);

        Console.Write("Enter file path to save (without extension): ");
        var filePath = Console.ReadLine();
        
        try
        {
            await File.WriteAllBytesAsync($"{filePath}.{selectedExporter.GetFileExtension()}", exportResult);
            Console.WriteLine($"Tasks exported successfully to {filePath}.{selectedExporter.GetFileExtension()}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting tasks: {ex.Message}");
        }
    }
}