namespace CommandLineProject.Options;

public static class VerbOptionHandler
{
    public static int RunCapitalizeAndReturnExitCode(CapitalizeOptions options)
    {
        Console.WriteLine(options.Capitalize);
        return 10;
    }

    public static int RunCommitAndReturnExitCode(CommitOptions options)
    {
        Console.WriteLine(options.Display);
        return 20;
    }

    public static int RunCloneAndReturnExitCode(TextOptions options)
    {
        Console.WriteLine(options.Text);
        return 30;
    }
}
