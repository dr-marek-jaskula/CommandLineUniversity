namespace CommandLiner.Common.Utilities;

public static class EnvironmentVariableUtilities
{
    public const string PATH = nameof(PATH);

    public static IDisposable SetEnvironmentVariable(string variableName, string? newValue)
    {
        var initialValue = Environment.GetEnvironmentVariable(variableName);
        Environment.SetEnvironmentVariable(variableName, newValue);

        return Disposable.Create(() => Environment.SetEnvironmentVariable(variableName, initialValue));
    }

    public static IDisposable ExtendPath(string input)
    {
        return SetEnvironmentVariable(PATH, $"{Environment.GetEnvironmentVariable(PATH)}{Path.PathSeparator}{input}");
    }
}