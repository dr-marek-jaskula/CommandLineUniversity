using CommandLine;
using CommandLineProject.Options;

var pathToProjectDirectory = AppDomain.CurrentDomain.BaseDirectory.Split(@"bin\", StringSplitOptions.None)[0];

var parser = CommandLine.Parser.Default;

return parser.ParseArguments<CapitalizeOptions, CommitOptions, TextOptions>(args).MapResult(
    (CapitalizeOptions options) => VerbOptionHandler.RunCapitalizeAndReturnExitCode(options),
    (CommitOptions options) => VerbOptionHandler.RunCommitAndReturnExitCode(options),
    (TextOptions options) => VerbOptionHandler.RunCloneAndReturnExitCode(options),
    errors => 1);