using sync.Helpers;
using sync.Logger;

namespace sync.DataManagement;

public class FolderSync(Config config,FileLogger log ,CancellationTokenSource cts)
{
public async Task ReadAsync()
{
    while (!cts.Token.IsCancellationRequested)
    {
        try
        {

        var files = Directory.GetFiles(config.SourceFolder);
        var targetFiles = Directory.GetFiles(config.TargetFolder);

        var fileNames = new HashSet<string?>(
            files.Select(Path.GetFileName)
        );

        var missingFiles = targetFiles
            .Where(f => !fileNames.Contains(Path.GetFileName(f)))
            .ToArray();

        if(missingFiles.Length > 0)
        {
            foreach (var file in missingFiles)
            {
                if (File.Exists(file))
                {
                    Console.WriteLine(Path.GetFileName(file));
                    File.Delete(file);
                    log.Log(
                        new()
                        {
                            FileName  = Path.GetFileName(file),
                            FileOperation = FileOperation.DELETE.ToString()
                        }
                    );   
                }   
            }
        }


            files = Directory.GetFiles(config.SourceFolder);
            foreach (var file in files)
            {
                cts.Token.ThrowIfCancellationRequested();

                var fileName = Path.GetFileName(file);
                var targetPath = Path.Combine(config.TargetFolder, fileName);

                if (File.Exists(targetPath))
                {
                    if (!Hash.CompareHash(file, targetPath))
                    {
                        File.Copy(file, targetPath, overwrite: true);
                        log.Log(new Log
                        {
                            FileName = fileName,
                            FileOperation = FileOperation.UPDATE.ToString()
                        });
                    }

                    continue;
                }

                File.Copy(file, targetPath, overwrite: false);

                log.Log(new Log
                {
                    FileName = fileName,
                    FileOperation = FileOperation.WRITE.ToString()
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            cts.Cancel();
            break;
        }

        await Task.Delay(
            TimeSpan.FromSeconds(config.TimeIntervalInSeconds),
            cts.Token
        );
    }
}
  
}