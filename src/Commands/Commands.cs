using sync.Enums;
using Spectre.Console;
using System.Text.Json;

namespace sync.src.Commands;
public class CommandsHandler(
    SetSourceFolderCommand setSourceFolderCommand,
    LoadConfigCommand loadConfigCommand,
    ReadLogsCommand readLogsCommand,
    CancellationTokenSource cts
) {
    public bool Stop {get; set;} = false;

   private Dictionary<CommandsEnum, CommandBase> _commands = new Dictionary<CommandsEnum, CommandBase>
    {
        { CommandsEnum.SOURCE, setSourceFolderCommand },
        { CommandsEnum.CONFIG, loadConfigCommand },
        { CommandsEnum.READ_LOGS, readLogsCommand }
    };

    public void RecieveCommand()
    {
        while (true)
            {
                cts.Token.ThrowIfCancellationRequested();
                var features = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("Select [green]Command[/] :")
                .AddChoices(
                    "Source", 
                    "Target", 
                    "Interval", 
                    "Log",
                    "Config",
                    "Read Logs",
                    "Exit"
                ));
        
                if (features != null)
                {
                    features = JsonNamingPolicy.SnakeCaseLower.ConvertName(features);
                    if(Enum.TryParse(features, ignoreCase: true, out CommandsEnum cmd))
                    {
                        if (cmd == CommandsEnum.EXIT)
                        {
                            cts.Cancel();
                            Console.WriteLine("Exiting application... bye bye!");
                            break;
                        }               
                        
                        if(_commands.TryGetValue(cmd, out var command))
                        {
                            command.RunCommand();        
                        }  
                    }
                    
                    // switch (cmd)
                    // {
                    //     case CommandsEnum.TARGET:
                    //         Console.Write("Enter target folder path: ");
                    //         conf.TargetFolder = Console.ReadLine() ?? conf.TargetFolder;
                    //         break;
                    //     case CommandsEnum.INTERVAL:
                    //         Console.Write("Enter interval (seconds): ");
                    //         if (int.TryParse(Console.ReadLine(), out int interval)) conf.TimeIntervalInSeconds = interval;
                    //         break;
                    //     case CommandsEnum.LOG:
                    //         Console.Write("Enter log file path: ");
                    //         conf.LogFilePath = Console.ReadLine() ?? "./logs";
                    //         break;
                    //     default:
                    //         Console.WriteLine("Command not recognized.");
                    //         break;
                    // }
                }
                
            }
    }
}