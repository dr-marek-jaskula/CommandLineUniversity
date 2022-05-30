using CommandLine;
using CommandLineProject.Options;

var pathToProjectDirectory = AppDomain.CurrentDomain.BaseDirectory.Split(@"bin\", StringSplitOptions.None)[0];

var parser = new CommandLine.Parser(with => with.HelpWriter = null);

var parserResult = parser.ParseArguments<Options>(args);

parserResult.WithParsed(options =>
{
    Console.WriteLine($"Verbose status: {options.Verbose}");
}).WithNotParsed(errors => HelpHandler.DisplayHelp(parserResult, errors));

Console.WriteLine("Hello, World!");

StreamReader sr = new("ReadMe.txt");

Console.WriteLine(sr.ReadToEnd());

Console.WriteLine("Bye bye, world");

return 0;