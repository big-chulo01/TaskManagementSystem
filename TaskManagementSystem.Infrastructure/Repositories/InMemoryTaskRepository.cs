using System.Collections.Concurrent;
using TaskManagementSystem.Domain.Entities;
using TaskManagementSystem.Domain.Interfaces;

namespace TaskManagementSystem.Infrastructure.Repositories;

public class InMemoryTaskRepository : ITaskRepository
{
    private readonly ConcurrentDictionary<int, TaskItem> _tasks = new();
    private int _nextId = 1;

    public async Task<TaskItem?> GetByIdAsync(int id)
    {
        _tasks.TryGetValue(id, out var task);
        return task;
    }

    public async Task<IEnumerable<TaskItem>> GetAllAsync()
    {
        return await Task.FromResult(_tasks.Values.AsEnumerable());
    }

    public async Task AddAsync(TaskItem task)
    {
        task.Id = _nextId++;
        _tasks.TryAdd(task.Id, task);
    }

    public async Task UpdateAsync(TaskItem task)
    {
        _tasks[task.Id] = task;
    }

    public async Task DeleteAsync(int id)
    {
        _tasks.TryRemove(id, out _);
    }
}