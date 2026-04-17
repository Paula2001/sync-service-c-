using sync.Commands;

namespace sync.DataManagement;

public class FolderSync(CommandsHandler commands, Config config)
{
    public Thread Read()
    {
        string folderPath = config.SourceFolder;

        Thread thread = new(() =>
        {
            while (true)
            {
                if(commands.Stop)
                {
                    Console.WriteLine("This is stop");
                    break;   
                }
                try
                {
                    var files = Directory.GetFiles(folderPath);

                    Console.WriteLine($"[{DateTime.Now}] Files:");
                    foreach (var file in files)
                    {
                        Console.WriteLine(Path.GetFileName(file));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                Thread.Sleep(config.TimeIntervalInSeconds); // 10 seconds delay
            }
        });

        thread.Start();

        return thread;
    }
}