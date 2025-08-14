using System.Runtime.Loader;
using System.Reflection;
using Exporter.Abstractions;

namespace TaskManagementSystem.Presentation;

public class PluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver _resolver;

    public PluginLoadContext(string pluginPath)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        return assemblyPath != null ? LoadFromAssemblyPath(assemblyPath) : null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        return libraryPath != null ? LoadUnmanagedDllFromPath(libraryPath) : IntPtr.Zero;
    }
}

public static class PluginLoader
{
    public static IEnumerable<IExporter> LoadPlugins(string pluginsDirectory)
    {
        if (!Directory.Exists(pluginsDirectory))
        {
            Directory.CreateDirectory(pluginsDirectory);
            return Enumerable.Empty<IExporter>();
        }

        var exporters = new List<IExporter>();

        foreach (var pluginPath in Directory.GetFiles(pluginsDirectory, "*.dll"))
        {
            try
            {
                var loadContext = new PluginLoadContext(pluginPath);
                var assembly = loadContext.LoadFromAssemblyPath(pluginPath);
                
                foreach (var type in assembly.GetTypes())
                {
                    if (typeof(IExporter).IsAssignableFrom(type) && !type.IsInterface)
                    {
                        if (Activator.CreateInstance(type) is IExporter exporter)
                        {
                            exporters.Add(exporter);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load plugin {Path.GetFileName(pluginPath)}: {ex.Message}");
            }
        }

        return exporters;
    }
}