using System.CommandLine;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Voting.Client;
using Voting.Server.Protos.v1;

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
        return await Task.FromResult(ConsoleCommands.RootCommand.InvokeAsync(args).Result);
    }
}