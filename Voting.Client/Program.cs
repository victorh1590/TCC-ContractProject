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

        //Options(flags)
        var jsonOption = new Option<FileInfo?>(
            name: "--json",
            description: "An option whose argument json file is parsed as a FileInfo.",
            isDefault: true, //Default value exists.
            parseArgument: result => // Validation.
            {
                if (result.Tokens.Count == 0)
                {
                    return null; // Default.
                }

                string? filePath = result.Tokens.SingleOrDefault()?.Value;
                FileInfo fi = new FileInfo(filePath ?? string.Empty);
                if (String.IsNullOrEmpty(fi.FullName) || !fi.Exists)
                {
                    result.ErrorMessage = "File does not exist.";
                    return null;
                }

                if (fi.Extension != "json")
                {
                    result.ErrorMessage = "File is not json.";
                    return null;
                }

                return fi;
            });

        var sectionOption = new Option<uint?>(
            name: "--section",
            description: "Sections to look for when querying the blockchain.",
            isDefault: true, //Default value exists.
            parseArgument: result => // Validation.
            {
                if (result.Tokens.Count == 0)
                {
                    return null; // Default.
                }

                string sectionString = result.Tokens.SingleOrDefault()?.Value ?? string.Empty;
                if (!uint.TryParse(sectionString, out uint section))
                {
                    result.ErrorMessage = "No section passed.";
                    return null;
                }

                return section;
            });

        // sectionOption.AddValidator(result =>
        // {
        //     if (result.Tokens.Count == 0 || result.GetValueForOption() == null)
        //     {
        //         result.ErrorMessage = "Must set a value for option --section, --candidate or both.";
        //     }
        // });

        var candidateOption = new Option<uint?>(
            name: "--candidate",
            description: "Candidate to look for when querying the blockchain.",
            isDefault: true, //Default value exists.
            parseArgument: result => // Validation.
            {
                // if (result.Tokens.Count == 0)
                // {
                //     result.ErrorMessage = "No candidate passed.";
                // }
                
                if (result.Tokens.Count == 0)
                {
                    return null; // Default.
                }

                string candidateString = result.Tokens.SingleOrDefault()?.Value ?? string.Empty;
                if (!uint.TryParse(candidateString, out uint candidate))
                {
                    result.ErrorMessage = "No candidate passed.";
                    return null;
                }

                return candidate; // Default
            });

        // candidateOption.AddValidator(result =>
        // {
        //     if (result.Tokens.Count == 0 || result.GetValueOrDefault<uint?>() == null)
        //     {
        //         result.ErrorMessage = "Must set a value for option --section, --candidate or both.";
        //     }
        // });

        //Create commands.
        //Root command.
        var rootCommand = new RootCommand("Sample app for System.CommandLine");

        //Subcommands
        var getCommand = new Command("get", "Get voting data from blockchain.")
        {
            sectionOption,
            candidateOption
        };
        getCommand.AddValidator(result =>
        {
            // if (result.Tokens.Count == 0)
            // {
            //     result.ErrorMessage = "Must set a value for option --section, --candidate or both.";
            // }
            // if (result.GetValueForOption(candidateOption) == null && result.GetValueForOption(sectionOption) == null)
            // {
            //     result.ErrorMessage = "Must set a value for option --section, --candidate or both.";
            // }
        });

        var totalCommand = new Command("total", "Get totalization of votes from blockchain.")
        {
            sectionOption,
            candidateOption
        };
        totalCommand.AddValidator(result =>
        {
            // if (result.Tokens.Count <= 0)
            // {
            //     result.ErrorMessage = "Must set a value for option --section, --candidate or both.";
            // }
        });

        var addCommand = new Command("add", "Send voting data to the blockchain.")
        {
            jsonOption
        };
        addCommand.AddValidator(result =>
        {
            // if (result.Tokens.Count <= 1)
            // {
            //     result.ErrorMessage = "Must set a value for at least one option (--json or --csv).";
            // }
        });
        addCommand.AddAlias("insert");
        addCommand.AddAlias("send");
        addCommand.AddAlias("put");

        rootCommand.AddCommand(getCommand);
        rootCommand.AddCommand(totalCommand);
        rootCommand.AddCommand(addCommand);

        //Handlers.
        getCommand.SetHandler(
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
            sectionOption, candidateOption);

        totalCommand.SetHandler(
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
            sectionOption, candidateOption);

        //Invoke root command.
        return await Task.FromResult(rootCommand.InvokeAsync(args).Result);
    }
}