using System.Reflection.Metadata;
using ColorLoggerLibrary;
using sync.Commands;
using sync.DataManagement;

namespace sync;

public sealed class Application(CommandsHandler commandsHandler, FolderSync folderSync)
{
    public void Run()
    {
        var t = new Thread(new ThreadStart(() => commandsHandler.RecieveCommand()));
        t.Start();
        
        var t2 = folderSync.Read();
        t.Join();
        t2.Join();
    }
}