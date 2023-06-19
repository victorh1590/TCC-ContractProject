using System.CommandLine;

namespace Voting.Client;

public static class ConsoleCommands
{
    public static RootCommand RootCommand { get; }
    private static Command GetCommand { get; }
    private static Command AddCommand { get; }
    private static Command TotalCommand { get; }
    // private static readonly ILogger _logger;
    
    //Error Messages.
    private static string SectionOrCandidateNotFound => "Section or candidate not found.";
    private static string SectionFailedToBeAdded 
        => "Section Failed to be added. Please verify your json file and make sure the sections are unique.";

    static ConsoleCommands()
    {
        // var loggerFactory = LoggerFactory.Create(logging =>
        // {
        //     logging.AddConsole();
        //     logging.SetMinimumLevel(LogLevel.Trace);
        // });
        //
        //Create commands.
        //Root command.
        RootCommand = new RootCommand("Sample app for System.CommandLine");

        //Subcommands
        GetCommand = new Command("get", "Get voting data from blockchain.")
        {
            ConsoleOptions.SectionOption,
            ConsoleOptions.CandidateOption
        };
        GetCommand.AddValidator(result =>
        {
            if (!result.Children.Any(opt => opt.Tokens.Count >= 1)) 
            {
                result.ErrorMessage = "Must set a value for option --section, --candidate or both.";
            }
        });

        TotalCommand = new Command("total", "Get totalization of votes from blockchain.")
        {
            ConsoleOptions.SectionOption,
            ConsoleOptions.CandidateOption
        };
        TotalCommand.AddValidator(result =>
        {
            if (!result.Children.Any(opt => opt.Tokens.Count >= 1)) 
            {
                result.ErrorMessage = "Must set a value for option --section, --candidate or both.";
            }
        });

        AddCommand = new Command("add", "Send voting data to the blockchain.")
        {
            ConsoleOptions.JsonOption
        };
        AddCommand.AddValidator(result =>
        {
            if (result.Children.Count < 1 && result.Children.FirstOrDefault()?.Tokens.Count == 0)
            {
                result.ErrorMessage = "Must set a value for --json or --csv.";
            }
        });
        AddCommand.AddAlias("insert");
        AddCommand.AddAlias("send");
        AddCommand.AddAlias("put");

        RootCommand.AddCommand(GetCommand);
        RootCommand.AddCommand(TotalCommand);
        RootCommand.AddCommand(AddCommand);
        
        //Handlers.
        GetCommand.SetHandler(
            async (section, candidate) =>
            {
                var client = new GrpcClient();
                try
                {
                    if (section != null && candidate != null)
                    {
                        throw new NotImplementedException("Not implemented");
                    }
                    else if (section != null)
                    {
                        await client.GetSectionVotes((uint)section);
                    }
                    else if (candidate != null)
                    {
                        await client.GetCandidateVotes((uint)candidate);
                    }
                    else
                    {
                        throw new Exception("No suitable method.");
                    }
                }
                catch
                {
                    WriteError(SectionOrCandidateNotFound);
                }
                finally
                {
                    client.Dispose();
                }
            },
            ConsoleOptions.SectionOption, ConsoleOptions.CandidateOption);

        TotalCommand.SetHandler(
            async (section, candidate) =>
            {
                var client = new GrpcClient();
                try
                {
                    if (section != null && candidate != null)
                    {
                        throw new NotImplementedException("Not implemented");
                    }
                    else if (section != null)
                    {
                        await client.GetTotalVotesBySection((uint)section);
                    }
                    else if (candidate != null)
                    {
                        await client.GetTotalVotesByCandidate((uint)candidate);
                    }
                    else
                    {
                        throw new Exception("No suitable method.");
                    }
                }
                catch
                {
                    WriteError(SectionOrCandidateNotFound);
                }
                finally
                {
                    client.Dispose();
                }
            },
            ConsoleOptions.SectionOption, ConsoleOptions.CandidateOption);
        
        AddCommand.SetHandler(
            async fi =>
            {
                var client = new GrpcClient();
                try
                {
                    if (fi != null)
                    {
                        using StreamReader sr = fi.OpenText();
                        string content = await sr.ReadToEndAsync();
                        
                        var result = await client.CreateSection(content);
                        Console.WriteLine(result);
                    }
                    else
                    {
                        throw new Exception("No suitable method.");
                    }
                }
                catch
                {
                    WriteError(SectionFailedToBeAdded);
                }
                finally
                {
                    client.Dispose();
                }
            },
            ConsoleOptions.JsonOption);
    }
    
    private static void WriteError(string errorMessage)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(errorMessage);
        Console.ResetColor();
    }
}