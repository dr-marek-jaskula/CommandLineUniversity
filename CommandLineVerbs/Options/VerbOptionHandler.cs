namespace CommandLineProject.Options;

public static class VerbOptionHandler
{
    public static int RunCapitalizeAndReturnExitCode(CapitalizeOptions options)
    {
        Console.WriteLine(options.Capitalize);
        Console.WriteLine(options.Message);
        return 10;
    }

    public static int RunCommitAndReturnExitCode(CommitOptions options)
    {
        Console.WriteLine(options.Display);
        Console.WriteLine(options.Message);
        return 20;
    }

    public static int RunCloneAndReturnExitCode(TextOptions options)
    {
        Console.WriteLine(options.Text);
        Console.WriteLine(options.Message);
        return 30;
    }
}