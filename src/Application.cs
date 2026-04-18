using sync.src.Commands;
using sync.DataManagement;

namespace sync;

public sealed class Application(CommandsHandler commandsHandler, FolderSync folderSync)
{
    public async Task Run()
    {
        await Task.WhenAll(
            Task.Run(() => commandsHandler.RecieveCommand()),
            folderSync.Read()
        );
    }
}