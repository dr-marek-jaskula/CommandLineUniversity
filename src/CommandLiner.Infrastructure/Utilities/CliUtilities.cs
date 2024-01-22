using CliWrap.Builders;

namespace CommandLiner.Infrastructure.Utilities;

public static class CliUtilities
{
    public static ArgumentsBuilder AddOption(this ArgumentsBuilder args, string name, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return args;
        }

        return args.Add(name).Add(value);
    }
}