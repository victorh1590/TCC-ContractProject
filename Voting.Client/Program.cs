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
        
        //Invoke root command.
        return await ConsoleCommands.RootCommand.InvokeAsync(args);
    }
}