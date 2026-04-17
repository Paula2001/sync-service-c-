namespace sync;

public class Config
{
    public string SourceFolder {get; set;} = "source";
    public string TargetFolder {get; set;} = "target";
    public string LogFilePath {get; set;} = "./logs";
    public int TimeIntervalInSeconds {get; set;} = 1;
}