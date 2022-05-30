using CommandLine;
using CommandLine.Text;
using CommandLineProject.Options;
using Microsoft.Extensions.Configuration;

var pathToProjectDirectory = AppDomain.CurrentDomain.BaseDirectory.Split(@"bin\", StringSplitOptions.None)[0];

var configuration = new ConfigurationBuilder()
    .SetBasePath(pathToProjectDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

//1- disable auto generated help
var parser = new CommandLine.Parser(with => with.HelpWriter = null);

//2- run parser and get result
var parserResult = parser.ParseArguments<Options>(args);

//3- generate help based on result and parameters
var helpText = HelpHandler.BuildHelp(parserResult);

parserResult.WithParsed(options =>
{
    if (options.Verbose)
    {
        Console.WriteLine($"Verbose output enabled. Current Arguments: -v {options.Verbose}");
        Console.WriteLine("Quick Start Example! App is in Verbose mode!");
    }
    else
    {
        Console.WriteLine($"Current Arguments: -v {options.Verbose}");
        Console.WriteLine("Quick Start Example!");
    }
}).WithNotParsed(errors => HelpHandler.DisplayHelp(parserResult, errors));

Console.WriteLine("Hello, World!");

StreamReader sr = new("ReadMe.txt");
Console.WriteLine(sr.ReadToEnd());

Console.WriteLine("Bye bye, world");

return CommandLine.Parser.Default.ParseArguments<CapitalizeOptions, CommitOptions, TextOptions>(args)
  .MapResult(
    (CapitalizeOptions options) => VerbOptionHandler.RunCapitalizeAndReturnExitCode(options),
    (CommitOptions options) => VerbOptionHandler.RunCommitAndReturnExitCode(options),
    (TextOptions options) => VerbOptionHandler.RunCloneAndReturnExitCode(options),
    errors => 1);