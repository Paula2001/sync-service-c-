 class Program
{
    // add tests to test the commands and test the sync and logs and the commands
    // Folder paths, synchronization interval and log file path should be provided using
    // the command line arguments;
    static void Main()
    {


        // Synchronization should be performed periodically; md5 to check what's updated and what's not when start

        // watching while running the project  listening to the system events
        //File creation/copying/removal operations should be logged to a file and to the
        // console output;
        var watcher = new FileSystemWatcher
        {
            Path = "/Users/paula/sync/source",
            NotifyFilter = NotifyFilters.FileName
                                 | NotifyFilters.DirectoryName
                                 | NotifyFilters.LastWrite,

            Filter = "*.*"
        };

        watcher.Created += (s, e) =>
            Console.WriteLine($"Created: {e.FullPath}");

        watcher.Deleted += (s, e) =>
            Console.WriteLine($"Deleted: {e.FullPath}");

        watcher.Changed += (s, e) =>
            Console.WriteLine($"Changed: {e.FullPath}");

        watcher.Renamed += (s, e) =>
            Console.WriteLine($"Renamed: {e.OldFullPath} -> {e.FullPath}");

        watcher.EnableRaisingEvents = true;

        Console.WriteLine("Watching...");
        Console.ReadLine();
    }
}