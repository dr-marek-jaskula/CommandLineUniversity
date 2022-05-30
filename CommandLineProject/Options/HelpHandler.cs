using CommandLine.Text;
using CommandLine;

namespace CommandLineProject.Options;

public class HelpHandler
{
    public static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errors)
    {
        var helpText = HelpText.AutoBuild(result, ht =>
        {
            ht.AdditionalNewLineAfterOption = false;
            ht.Heading = "ConsApp 1.0.0-beta";
            ht.Copyright = "Copyright (c) 2022 dr-marek-jaskula";
            return HelpText.DefaultParsingErrorsHandler(result, ht);
        }, e => e);
        Console.WriteLine(helpText);

        foreach (var error in errors)
            Console.WriteLine(error.Tag);

        Environment.Exit(1);
    }
}