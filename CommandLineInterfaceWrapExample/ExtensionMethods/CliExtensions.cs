using CliWrap.Builders;

namespace CommandLineInterfaceWrapExample.ExtensionMethods;

public static class CliExtensions
{
    public static ArgumentsBuilder AddOption(
        this ArgumentsBuilder args,
        string name,
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return args;

        return args.Add(name).Add(value);
    }
}