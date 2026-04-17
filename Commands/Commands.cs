using ColorLoggerLibrary;
using sync.Enums;
using System.Text;

namespace sync.Commands;
public class CommandsHandler(Config conf) {
    public bool Stop {get; set;} = false;
    public static void PrintInit()
    {
        StringBuilder sb = new();
        sb.AppendLine("FILE SYNC TOOL");
        sb.AppendLine();
        sb.AppendLine("1. Set source folder path");
        sb.AppendLine("2. Set target folder path");
        sb.AppendLine("3. Set synchronization interval (seconds)");
        sb.AppendLine("4. Set log file path");
        sb.AppendLine("4. Set log file path");
        sb.AppendLine("5. Show current configuration");
        sb.AppendLine("6. Start synchronization");
        sb.AppendLine("7. Exit");
        sb.AppendLine();
        sb.Append("Select an option (1-7): ");

        Console.WriteLine(sb.ToString());
    }

    public void RecieveCommand()
    {
        PrintInit();
        while (true)
            {
                string? consoleLine = Console.ReadLine();
                if (consoleLine != null)
                {
                    var result = Enum.TryParse(consoleLine, ignoreCase: true, out CommandsEnum cmd);
                    
                    if (cmd == CommandsEnum.EXIT)
                    {
                        Stop = true;
                        Console.WriteLine("Exiting application... bye bye!");
                        break;
                    }               
                
                    switch (cmd)
                    {
                        case CommandsEnum.SOURCE:
                            conf.SourceFolder = Console.ReadLine() ?? conf.SourceFolder;
                            break;
                        case CommandsEnum.TARGET:
                            Console.Write("Enter target folder path: ");
                            conf.TargetFolder = Console.ReadLine() ?? conf.TargetFolder;
                            break;
                        case CommandsEnum.INTERVAL:
                            Console.Write("Enter interval (seconds): ");
                            if (int.TryParse(Console.ReadLine(), out int interval)) conf.TimeIntervalInSeconds = interval;
                            break;
                        case CommandsEnum.LOG:
                            Console.Write("Enter log file path: ");
                            conf.LogFilePath = Console.ReadLine() ?? "./logs";
                            break;
                        case CommandsEnum.CONFIG:
                            Console.WriteLine("\n--- Current Configuration ---");
                            Console.WriteLine($"Source: {conf.SourceFolder}");
                            Console.WriteLine($"Target: {conf.TargetFolder}");
                            Console.WriteLine($"Interval: {conf.TimeIntervalInSeconds}s");
                            Console.WriteLine($"Log Path: {conf.LogFilePath}\n");
                            break;
                        case CommandsEnum.START:
                            Console.WriteLine("Starting synchronization...");
                            // Sync logic would go here
                            break;
                        default:
                            Console.WriteLine("Command not recognized.");
                            break;
                    }
                    Console.Write("Select an option (1-7): ");
                }
                
            }
    }
}