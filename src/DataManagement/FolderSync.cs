namespace sync.DataManagement;

public class FolderSync(Config config, CancellationTokenSource cts)
{
    public Task Read()
    {
        // TODO: I want to add a decrator arround this
        string sourceFolder = string.Concat(Config.DataFolder, config.SourceFolder);
        string targetFolder = string.Concat(Config.DataFolder, config.TargetFolder);
        string logsFile = string.Concat(Config.DataFolder, config.LogFilePath);
        return Task.Run(() =>
        {
            while (true)
            {
                cts.Token.ThrowIfCancellationRequested();
                try
                {
                    var files = Directory.GetFiles(sourceFolder);

                    // Console.WriteLine($"[{DateTime.Now}] Files:");
                    foreach (var file in files)
                    {
                        // Console.WriteLine(file);
                        // TODO: if the file name doesn't exist then move
                        // TODO: if the file name exists check with the md5 content if not equal then replace
                        // TODO: in both cases log it
                        // Console.WriteLine(Path.GetFileName(file));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    cts.Cancel();
                }
                Thread.Sleep(config.TimeIntervalInSeconds);
            }
        });
    }
}