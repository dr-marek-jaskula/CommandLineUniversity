using CommandLine;

namespace CommandLineProject.Options;

public class Options
{
    [Option('o', "output", Required = false, Default = "example", HelpText = "Enter the file name.")]
    public string fileName { get; set; } = string.Empty;

    [Option('r', "read", Required = true, HelpText = "Input files to be processed.")]
    public IEnumerable<string> InputFiles { get; set; } = new List<string>();

    // Omitting long name, defaults to name of property, i.e. "--verbose". If "--verbose" is present, then it is true, if not it is false
    [Option(Default = false, HelpText = "Prints all messages to standard output.")]
    public bool Verbose { get; set; }

    [Value(0, MetaName = "offset", HelpText = "File offset.")]
    public long? Offset { get; set; }
}