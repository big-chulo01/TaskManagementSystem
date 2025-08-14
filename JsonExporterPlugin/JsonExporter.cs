using Exporter.Abstractions;
using System.Text.Json;
using TaskManagementSystem.Domain.Entities;

namespace JsonExporterPlugin;

public class JsonExporter : IExporter
{
    public string GetFormatName() => "JSON";
    
    public string GetFileExtension() => "json";
    
    public byte[] Export(IEnumerable<TaskItem> tasks)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(tasks, options);
        return System.Text.Encoding.UTF8.GetBytes(json);
    }
}