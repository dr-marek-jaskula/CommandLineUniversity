using CommandLine;

namespace CommandLineProject.Options;

public class Options
{
    [Option('m', "message", Required = false, Default = "Hello", HelpText = "Message to display.")]
    public string Message { get; set; } = string.Empty;
}

[Verb("add", HelpText = "Add file contents to the index.")]
public class CapitalizeOptions : Options
{
    [Option('c', "capitalize", Required = false, Default = false, HelpText = "Capitalize boolean option.")]
    public bool Capitalize { get; set; }
}

[Verb("commit", HelpText = "Record changes to the repository.")]
public class CommitOptions : Options
{
    [Option('d', "display", Required = false, Default = false, HelpText = "Display boolean option.")]
    public bool Display { get; set; }
}

[Verb("clone", HelpText = "Clone a repository into a new directory.")]
public class TextOptions : Options
{
    [Option('t', "text", Required = false, Default = "Default Text", HelpText = "Text to console write line.")]
    public string Text { get; set; } = string.Empty;
}