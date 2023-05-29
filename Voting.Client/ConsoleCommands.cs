using System.CommandLine;
using System.Linq;

namespace Voting.Client;

public static class ConsoleCommands
{
    public static RootCommand RootCommand { get; }
    public static Command GetCommand { get; }
    public static Command AddCommand { get; }
    public static Command TotalCommand { get; }

    static ConsoleCommands()
    {
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
            if (result.Tokens.Count == 0 || result.ErrorMessage?.Length >= 1)
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
            if (result.Tokens.Count == 0 || result.ErrorMessage?.Length >= 1)
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
            if (result.Tokens.Count == 0 || result.ErrorMessage?.Length >= 1)
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
                if (section != null && candidate != null)
                {
                    throw new NotImplementedException("Not implemented");
                }
                else if (section != null)
                {
                    // await ReadFile(file!, delay, fgcolor, lightMode);
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

                client.Dispose();
            },
            ConsoleOptions.SectionOption, ConsoleOptions.CandidateOption);

        TotalCommand.SetHandler(
            async (section, candidate) =>
            {
                var client = new GrpcClient();
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

                client.Dispose();
            },
            ConsoleOptions.SectionOption, ConsoleOptions.CandidateOption);
    }
}