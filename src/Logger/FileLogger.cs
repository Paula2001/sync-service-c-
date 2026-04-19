using System.Text.Json;
using System.Threading.Channels;

namespace sync.Logger;

public class FileLogger
{
    private readonly Config _config;

    private readonly Channel<Log> _channel;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _consumerTask;

    public FileLogger(Config config)
    {
        _config = config;

        _channel = Channel.CreateUnbounded<Log>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        _consumerTask = Task.Run(ProcessQueueAsync);
    }

    public void Log(Log log)
    {
        _channel.Writer.WriteAsync(log);
    }

    private async Task ProcessQueueAsync()
    {
        await foreach (var log in _channel.Reader.ReadAllAsync(_cts.Token))
        {
            await WriteLogAsync(log);
        }
    }

    private async Task WriteLogAsync(Log log)
    {
        Dictionary<string, Timestamp> data;

        if (File.Exists(_config.LogFilePath))
        {
            await using var stream = File.OpenRead(_config.LogFilePath);
            data = await JsonSerializer.DeserializeAsync<Dictionary<string, Timestamp>>(stream)
                   ?? new();
        }
        else
        {
            data = new();
        }

        data[DateTime.UtcNow.ToString("O")] = new Timestamp
        {
            FileName = log.FileName,
            Operation = log.FileOperation
        };

        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(_config.LogFilePath, json);
    }

    public async Task<string> ReadLogsAsync()
    {
        if (!File.Exists(_config.LogFilePath))
            return "{}";

        return await File.ReadAllTextAsync(_config.LogFilePath);
    }

    public async Task StopAsync()
    {
        _channel.Writer.Complete();
        _cts.Cancel();

        await _consumerTask;
    }
}