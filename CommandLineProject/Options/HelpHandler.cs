using CommandLine.Text;
using CommandLine;

namespace CommandLineProject.Options;
public class HelpHandler
{
    public static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errors)
    {
        HelpText? helpText = null;

        if (errors.IsVersion())
            helpText = HelpText.AutoBuild(result);
        else
        {
            foreach (var error in errors)
            {
                Console.WriteLine(error.Tag);
                if (!error.StopsProcessing)
                    Environment.Exit(0);
            }

            helpText = HelpText.AutoBuild(result, h =>
            {
                //configure help
                h.AdditionalNewLineAfterOption = false;
                h.Heading = "ConsApp 1.0.0-beta";
                h.Copyright = "Copyright (c) 2022 dr-marek-jaskula";
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
        }

        Console.WriteLine(helpText);
    }

    public static HelpText BuildHelp(ParserResult<Options> parserResult)
    {
        return HelpText.AutoBuild(parserResult, ht =>
        {
            //configure HelpText
            ht.AdditionalNewLineAfterOption = false; //remove newline between options
            ht.Heading = "ConsApp 1.0.0-beta";
            ht.Copyright = "Copyright (c) 2022 dr-marek-jaskula";
            return ht;
        }, e => e);
    }
}
