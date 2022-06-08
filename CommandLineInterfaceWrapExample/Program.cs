using CliWrap;
using CliWrap.Buffered;
using CliWrap.EventStream;
using CommandLineInterfaceWrapExample.ExtensionMethods;
using System.Text;

//Agenda of good approaches:
//4 (proper argument pass),
//5 (custom extension methods),
//6 (Streaming -> IAsyncEnumerable and await foreach)
//8 (proper piping)
//9 (cancellation tokens)
//11 (Process multiple command -> move results from one to one and finally to program). Very important.

#region Basics

//1. Basic execution
await Cli
    //command that will be executed
    .Wrap("docker")
    //arguments in form of a string that will be placed after the "docker" stirng
    .WithArguments("run --detach -e POSTGRES_PASSWORD=password postgres")
    //async execution without the all output (like all information lines)
    .ExecuteAsync();
//Show contents of command result
//Show how to get process ID

//2. Buffered execution
var result = await Cli.Wrap("docker")
    .WithArguments("run --detach -e POSTGRES_PASSWORD=password postgres")
    //now the results from the operation are stored in "result" variable
    .ExecuteBufferedAsync();
//Show contents of buffered command result

//3. Execution that fails
await Cli.Wrap("docker")
    //Intentionally write the wrong image name "postresss"
    .WithArguments("run --detach -e POSTGRES_PASSWORD=password postgresss")
    .ExecuteAsync();
//Show how ExecuteBufferedAsync() affects the error message

#endregion Basics

#region 4. Passing arguments in different ways

//a) String interpolation
var password = "hello world";
var result2 = await Cli.Wrap("docker")
    //BAD APPOROACH!! DO NOT use the string interpolation here!!
    .WithArguments($"run --detach -e POSTGRES_PASSWORD={password} postgres")
    .ExecuteBufferedAsync();
//Show that the password is not escaped and the arguments end up malformed
//Instead of password, it can be anything, e.g. file path

//Solution to this problem:
//b) use array
var result3 = await Cli.Wrap("docker")
    //Pass arguments as a array
    .WithArguments(new[]
    {
        "run",
        "--detach",
        "-e", $"POSTGRES_PASSWORD={password}",
        "postgres"
    })
    .ExecuteBufferedAsync();
//Show that arguments are escaped correctly
//Nevertheless, this is not the best approach.

//Best way, using ArgumentBuilder:
//c) ArgumentBuilder
var result4 = await Cli.Wrap("docker")
    .WithArguments(args => args
        .Add("run")
        .Add("--detach")
        .Add("-e").Add($"POSTGRES_PASSWORD={password}")
        .Add("postgres")
    )
    .ExecuteBufferedAsync();
//Show usage with IFormattable

#endregion 4. Passing arguments in different ways

#region 5. Custom arguments builder extensions

var network = "mynet";

var result5 = await Cli.Wrap("docker")
    .WithArguments(args => args
        .Add("run")
        .Add("--detach")
        .Add("-e").Add("POSTGRES_PASSWORD=hello world")
        //We use the extension method in "ExtensionMethods" folder, to add --network only when network is not null
        .AddOption("--network", network)
        .Add("postgres")
    )
    .ExecuteBufferedAsync();
//Show how changing network to empty string does not pass the option

#endregion 5. Custom arguments builder extensions

#region 6. Event stream execution models

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

//Show how the events are handled as the process is running
//Explain backpressure and single-threaded event flow
//Show observable stream execution model
//Mention that you can easily create your own execution models

#endregion 6. Event stream execution models

#region 7. Output and error piping

//a) Way that is not good not bad
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

//Explain that this is functionally equivalent to ExecuteBufferedAsync() (exception non - zero exit code exception message)
//Show PipeTarget contract

//b) passing the result to the delegate (interesting)

await Cli.Wrap("docker")
    .WithArguments(args => args
        .Add("run")
        .Add("--detach")
        .Add("-e").Add("POSTGRES_PASSWORD=hello world")
        .Add("postgres")
    )
    .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
    .WithStandardErrorPipe(PipeTarget.ToDelegate(Console.WriteLine))
    .ExecuteAsync();

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

Console.WriteLine(result.StandardOutput);

//Explain that ListenAsync() and ObserveAsync() are using a similar setup under the hood
//Show that this supports async delegates too (Func<string, Task>)

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

#endregion 7. Output and error piping

#region 8. Output and error piping with OVERRIDDEN OPERATOR '|' (Interesting)

//The operator '|' is the operator that gives us the pipeline order

var cmd2 = Cli.Wrap("docker")
    .WithArguments(args => args
        .Add("run")
        .Add("-e").Add("POSTGRES_PASSWORD=hello world")
        .Add("postgres"))
    | (Console.WriteLine, Console.Error.WriteLine); //here the tuple is used:
//the first Console.WriteLine is for output and the second for the Error
//If there is one (not a tuple) the output is processed

await cmd2.ExecuteAsync();

#endregion 8. Output and error piping with OVERRIDDEN OPERATOR '|' (Interesting)

#region 9. Cancellation tokens

//Explain how CliWrap kills the process on cancellation

using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(6));

try
{
    await Cli.Wrap("ping")
        .WithArguments("-t google.com")
        .WithStandardOutputPipe(PipeTarget.ToDelegate(Console.WriteLine))
        .ExecuteAsync(cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Cancelled!");
}

#endregion 9. Cancellation tokens

#region 10. Input piping

//Remind that the same can be done using WithStandardInputPipe(...) instead
var cmd3 = PipeSource.FromFile("video.mp4") | Cli.Wrap("ffmpeg")
    .WithArguments(args => args
        .Add("-i").Add("-")
        .Add("video_out.webm"))
    | (Console.WriteLine, Console.Error.WriteLine);

await cmd3.ExecuteAsync();

//Other example
//var youtube = new YoutubeClient();
//var streamManifest = await youtube.Videos.Streams.GetManifestAsync("https://www.youtube.com/watch?v=-Sf9NPQeZOQ");
//var streamInfo = streamManifest.GetVideoStreams().GetWithHighestVideoQuality();
//var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

//var cmd = stream
//    | Cli.Wrap("ffmpeg")
//        .WithArguments(args => args
//            .Add("-i").Add("-")
//            .Add("-preset").Add("ultrafast")
//            .Add("youtube_video.mp4"))
//    | (Console.WriteLine, Console.Error.WriteLine);

//await cmd.ExecuteAsync();

#endregion 10. Input piping

#region 11. Process-to-process piping (Interesting)

var cmd4 =
    Cli.Wrap("ffmpeg") // Take first 5 seconds of the video and convert to webm
        .WithArguments(args => args
            .Add("-i").Add("video.mp4")
            .Add("-t").Add(5)
            .Add("-f").Add("webm")
            .Add("-"))
    //use the result from previous process to the next process!
    | Cli.Wrap("ffmpeg") //Reverse the stream and write to file
        .WithArguments(args => args
            .Add("-i").Add("-")
            .Add("-vf").Add("reverse")
            .Add("video_altered.webm"))
    //use the result to the delegates (output and error message)
    | (Console.WriteLine, Console.Error.WriteLine);

await cmd4.ExecuteAsync();

#endregion 11. Process-to-process piping (Interesting)