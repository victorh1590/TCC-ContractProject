using System.CommandLine;
using System.Threading.Tasks;
using Voting.Client;

namespace voting;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        //grpc Client
        // var loggerFactory = LoggerFactory.Create(logging =>
        // {
        //     logging.AddConsole();
        //     logging.SetMinimumLevel(LogLevel.Trace);
        // });

        

        //Invoke root command.
        return await ConsoleCommands.RootCommand.InvokeAsync(args);
    }
}