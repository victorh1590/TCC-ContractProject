using System.CommandLine;

namespace voting;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
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

        var candidateOption = new Option<uint?>(
            name: "--section",
            description: "Candidate to look for when querying the blockchain.",
            isDefault: true, //Default value exists.
            parseArgument: result => // Validation.
            {
                string candidateString = result.Tokens.SingleOrDefault()?.Value ?? string.Empty;
                if(!uint.TryParse(candidateString, out uint candidate))
                {
                    result.ErrorMessage = "No candidate passed.";
                    return null;
                }

                return candidate; // Default
            });

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
            if (result.Tokens.Count <= 1)
            {
                result.ErrorMessage = "Must set a value for option --section, --candidate or both.";
            }
        });
        
        var totalCommand = new Command("total", "Get totalization of votes from blockchain.")
        {
            sectionOption,
            candidateOption
        };
        totalCommand.AddValidator(result =>
        {
            if (result.Tokens.Count <= 1)
            {
                result.ErrorMessage = "Must set a value for option --section, --candidate or both.";
            }
        });
        
        var addCommand = new Command("add", "Send voting data to the blockchain.")
        {
            jsonOption
        };
        addCommand.AddValidator(result =>
        {
            if (result.Tokens.Count <= 1)
            {
                result.ErrorMessage = "Must set a value for at least one option (--json or --csv).";
            }
        });
        addCommand.AddAlias("insert");
        addCommand.AddAlias("send");
        addCommand.AddAlias("put");
        
        rootCommand.AddCommand(getCommand);
        rootCommand.AddCommand(totalCommand);
        rootCommand.AddCommand(addCommand);

        //Invoke root command.
        return await rootCommand.InvokeAsync(args);
    }
}