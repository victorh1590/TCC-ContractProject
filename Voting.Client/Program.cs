using System.CommandLine;
using System.CommandLine.Builder;
using System.Threading.Tasks;
using Voting.Client;

namespace voting;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        var _ = new CommandLineBuilder(ConsoleCommands.RootCommand)
            // .UseVersionOption()
            .UseHelp()
            .UseEnvironmentVariableDirective()
            .UseParseDirective()
            .UseSuggestDirective()
            .RegisterWithDotnetSuggest()
            .UseTypoCorrections()
            .UseParseErrorReporting()
            .CancelOnProcessTermination()
            .Build();

        // rootCommand.Invoke(args);

        try
        {
            return await ConsoleCommands.RootCommand.InvokeAsync(args);

        }
        catch (Exception e)
        {
            Console.WriteLine($"{e.Message}");
            return 0;
        }
        //Invoke root command.
    }
}