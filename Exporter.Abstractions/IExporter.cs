using TaskManagementSystem.Domain.Entities;

namespace Exporter.Abstractions;

public interface IExporter
{
    string GetFormatName();
    string GetFileExtension();
    byte[] Export(IEnumerable<TaskItem> tasks);
}