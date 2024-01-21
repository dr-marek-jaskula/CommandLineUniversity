using CliWrap;
using CliWrap.Buffered;
using CliWrap.EventStream;
using CommandLiner.Infrastructure.Utilities;
using System.Text;

namespace CommandLiner.Infrastructure.Services;

public sealed class CliService
{
    public async Task UseCli()
    {
        using var forcefulCts = new CancellationTokenSource();
        using var gracefulCts = new CancellationTokenSource();

        // Graceful cancel after a timeout of 7 seconds.
        gracefulCts.CancelAfter(TimeSpan.FromSeconds(7));

        // Force cancel after a timeout of 10 seconds.
        forcefulCts.CancelAfter(TimeSpan.FromSeconds(10));

        var network = "mynet";
        var result5 = await Cli.Wrap("docker")
            .WithWorkingDirectory("c:/projects/my project/")
            .WithEnvironmentVariables(env => env
                .Set("GIT_AUTHOR_NAME", "John")
                .Set("GIT_AUTHOR_EMAIL", "john@email.com"))
            .WithArguments(args => args
                .Add("run")
                .Add("--detach")
                .Add("-e").Add("POSTGRES_PASSWORD=hello world")
                .AddOption("--network", network)
                .Add("postgres"))
            .ExecuteAsync(forcefulCts.Token, gracefulCts.Token);
    }

    public async Task UseCli2()
    {
        await using var input = File.OpenRead("input.txt");
        await using var output = File.Create("output.txt");

        await Cli.Wrap("foo")
            .WithStandardInputPipe(PipeSource.FromStream(input))
            .WithStandardOutputPipe(PipeTarget.ToStream(output))
            .ExecuteBufferedAsync();
    }

    public async Task UseCli2()
    {
        await using var input = File.OpenRead("input.txt");
        await using var output = File.Create("output.txt");

        await Cli.Wrap("foo")
            .WithStandardInputPipe(PipeSource.FromStream(input))
            .WithStandardOutputPipe(PipeTarget.ToStream(output))
            .ExecuteBufferedAsync();
    }

    public async Task UseCli3()
    {

        //We use streaming (so IAsyncEnumerable with await foreach, to yield return asynchronously)
        var cmd = Cli.Wrap("docker")
            .WithArguments(args => args
                .Add("run")
                .Add("-e").Add("POSTGRES_PASSWORD=hello world")
                .Add("postgres"));

        //ListenAsync method has yield return and IAsyncEnumerable return type, so we results can be obtained one by one in async manner
        await foreach (var cmdEvent in cmd.ListenAsync())
        {
            switch (cmdEvent)
            {
                case StartedCommandEvent started:
                    {
                        Console.WriteLine("Process started. PID: " + started.ProcessId);
                        break;
                    }

                case StandardOutputCommandEvent stdOut:
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine("OUT> " + stdOut.Text);
                        Console.ResetColor();
                        break;
                    }

                case StandardErrorCommandEvent stdErr:
                    {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.WriteLine("ERR> " + stdErr.Text);
                        Console.ResetColor();
                        break;
                    }

                case ExitedCommandEvent exited:
                    {
                        Console.WriteLine("Process exited. Code: " + exited.ExitCode);
                        break;
                    }
            }
        }
    }

    public async Task UseClie4()
    {
        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        await Cli.Wrap("docker")
            .WithArguments(args => args
                .Add("run")
                .Add("--detach")
                .Add("-e").Add("POSTGRES_PASSWORD=hello world")
                .Add("postgres")
            )
            //The output will be processed to the StringBuilder
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            //The error result will be process to the StringBuilder
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
            .ExecuteAsync();

        Console.WriteLine(stdOutBuffer);
        Console.WriteLine(stdErrBuffer);
    }

    public async Task UseClie5()
    {
        //And with the result in the end
        var result6 = await Cli.Wrap("docker")
            .WithArguments(args => args
                .Add("run")
                .Add("--detach")
                .Add("-e").Add("POSTGRES_PASSWORD=hello world")
                .Add("postgres")
            )
            .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.WriteLine))
            .ExecuteBufferedAsync();

        Console.WriteLine(result6.StandardOutput);
    }

    public async Task UseClie6()
    {
        await Cli.Wrap("docker")
            .WithArguments(args => args
                .Add("run")
                .Add("--detach")
                .Add("-e").Add("POSTGRES_PASSWORD=hello world")
                .Add("postgres")
            )
            .WithStandardOutputPipe(PipeTarget.ToFile("stdout.txt"))
            .WithStandardErrorPipe(PipeTarget.ToFile("stderr.txt"))
            .ExecuteAsync();
    }
}