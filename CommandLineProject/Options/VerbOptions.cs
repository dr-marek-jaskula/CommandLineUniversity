using CommandLine;

namespace CommandLineProject.Options;

[Verb("add", HelpText = "Add file contents to the index.")]
public class CapitalizeOptions
{
    //normal options here
    [Option('c', "capitalize", Required = false, Default = false, HelpText = "Capitalize boolean option.")]
    public bool Capitalize { get; set; }
}

[Verb("commit", HelpText = "Record changes to the repository.")]
public class CommitOptions
{
    //commit options here
    [Option('d', "display", Required = false, Default = false, HelpText = "Display boolean option.")]
    public bool Display { get; set; }
}

[Verb("clone", HelpText = "Clone a repository into a new directory.")]
public class TextOptions
{
    //clone options here
    [Option('t', "text", Required = false, Default = "Default Text", HelpText = "Text to console write line.")]
    public string Text { get; set; } = string.Empty;
}