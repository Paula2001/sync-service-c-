# sync

Single-direction folder sync CLI built with .NET. The app runs two loops in parallel:
an interactive options menu and a polling sync worker that copies files from source
to replica and writes JSON logs.

This README is contributor-focused and documents current behavior only.

## Prerequisites

- .NET SDK 10.0 (project targets `net10.0`)
- `make`

## Quick Start

From the repository root:

```bash
make run
```

This starts the app using `dotnet run --project ./src/sync.csproj`.

Run tests:

```bash
make test
```

This executes `dotnet test ./Tests/Tests.csproj`.

## Makefile Reference

Current targets:

- `run`: starts the interactive application from `./src/sync.csproj`
- `test`: runs the test project at `./Tests/Tests.csproj`

There are no `build`, `clean`, or default/help targets yet.

## Runtime Model

The entrypoint creates a DI container and starts `Application.Run()`.

- `src/Program.cs`: creates `CancellationTokenSource`, builds services via `Bootstrap`, runs app
- `src/Application.cs`: runs options menu and folder sync concurrently via `Task.WhenAll`

Concurrency shape:

1. Options loop waits for interactive commands.
2. Sync loop scans source folder every configured interval.
3. Either loop can cancel the shared token to stop the app.

## Dependency Injection and Registration Pattern

`src/Bootstrap.cs` builds the service provider and registers core services as singletons.

Important pattern:

- All non-abstract subclasses of `OptionBase` are discovered with reflection and auto-registered.
- `OptionsHandler` receives concrete option classes via constructor injection.

This is the extension point for adding new menu features.

## Options Pattern

The options system is command-like:

- `src/Options/OptionBase.cs`: abstract base class with `Execute()` and shared `RunOption()` wrapper
- `src/Options/Options.cs`: menu selection and mapping from `OptionsEnum` to option instances

Current option classes:

- `SetSourceFolderOption`
- `SetTargetFolderOption`
- `SetIntervalOption`
- `SetLogPathOption`
- `LoadConfigOption`
- `ReadLogsOption`

### How Menu Selection Works

`OptionsHandler` prompts with Spectre Console labels like "Read Logs", converts selection
to snake_case, and parses it into `OptionsEnum`. `EXIT` triggers cancellation.

## Configuration Pattern

`src/Config.cs` holds mutable runtime settings shared by both loops.

Defaults:

- `SourceFolder`: `./data/source`
- `TargetFolder`: `./data/replica`
- `LogFilePath`: `./logs/logs.json`
- `TimeIntervalInSeconds`: `1`

Path properties compose `./data` and `./logs` prefixes in the getters.

## Sync Algorithm

`src/DataManagement/FolderSync.cs` implements polling-based, one-way replication.

Per cycle:

1. Enumerate files in source folder.
2. For each file, resolve matching path in replica.
3. If target exists, compare MD5 hashes (`src/Helpers/Hash.cs`).
4. Copy and log `UPDATE` when hashes differ.
5. Copy and log `WRITE` when file is missing in target.
6. Wait configured interval and repeat.

Error behavior:

- Exceptions are printed to console.
- Cancellation token is canceled and loop exits.

## Logging Format and Flow

Logging is file-based JSON in `src/Logger/FileLogger.cs`.

- Log records are modeled in `src/Logger/Log.cs`.
- Stored structure is `LogFile` dictionary (`src/Logger/LogFile.cs`) keyed by timestamp string.
- `ReadLogsAsync()` creates the log file if missing and returns raw JSON content.

Operation values currently used by sync logic:

- `WRITE`
- `UPDATE`

`DELETE` exists in `src/DataManagement/FileOperation.cs` but is not used by the current sync loop.

## Project Structure

Key directories:

- `src/`: application code
- `src/Options/`: interactive option handlers and menu orchestration
- `src/DataManagement/`: sync loop and operation enum
- `src/Logger/`: log models and persistence
- `Tests/`: xUnit tests and testing helpers

## Testing

Test project:

- `Tests/Tests.csproj`

Current examples:

- `Tests/LoadConfigOptionOutputTests.cs`: integration-style console output assertion
- `Tests/UnitTest1.cs`: placeholder unit test

## Extending the App

To add a new option:

1. Create a new class in `src/Options/` that inherits `OptionBase` and implements `Execute()`.
2. Add a corresponding enum value in `src/Enums/OptionsEnum.cs`.
3. Add the menu label in `OptionsHandler.ReceiveOption()`.
4. Inject the new option class into `OptionsHandler` constructor and add it to `_options` mapping.

Because `Bootstrap` auto-registers `OptionBase` subclasses, no manual DI registration is needed for the option class itself.

## Known Limitations (Current Behavior)

- Sync is one-way: source -> replica only.
- Source deletions are not propagated to replica.
- Polling model (interval-based) is used, not file-system watching.
- Shared mutable config is accessed concurrently by options and sync loops.
- `FileLogger.Log` is `async void`, which is harder to observe and test.
