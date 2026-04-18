using ColorLoggerLibrary;
using Microsoft.Extensions.DependencyInjection;
using sync.src.Commands;
using sync.DataManagement;

namespace sync;
class Program
{

    static async Task Main()
    {
        ServiceCollection services = new();
        
        typeof(CommandBase).Assembly
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(CommandBase)) && !t.IsAbstract)
            .ToList()
            .ForEach(t => services.AddSingleton(t));
        
        services.AddSingleton<CancellationTokenSource>();
        services.AddSingleton<CommandsHandler>();
        services.AddSingleton<Config>();
        services.AddSingleton<FolderSync>();
        services.AddSingleton<ColorLogger>();
        services.AddSingleton<Application>();

        // Build the provider
        var provider = services.BuildServiceProvider();

        // Run the app
        var app = provider.GetRequiredService<Application>();
        try
        {
            await app.Run();
        }
        catch (OperationCanceledException) { }
        finally
        {
            Environment.Exit(0);
        }
    }

}


