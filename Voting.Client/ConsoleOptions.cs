using System.CommandLine;
using System.IO;
using System.Linq;

namespace Voting.Client;

public static class ConsoleOptions
{
    public static Option<FileInfo?> JsonOption { get; }
    public static Option<uint?> SectionOption { get; }
    public static Option<uint?> CandidateOption { get; }

    static ConsoleOptions()
    {
        //Options(flags)
        JsonOption = new Option<FileInfo?>(
            name: "--json",
            description: "An option whose argument json file is parsed as a FileInfo.",
            isDefault: true, //Default value exists.
            parseArgument: result => // Validation.
            {
                if (!result.Tokens.Any())
                {
                    return null; // Default.
                }

                string? filePath = result.Tokens.SingleOrDefault()?.Value;
                FileInfo fi = new FileInfo(filePath ?? string.Empty);
                if (string.IsNullOrEmpty(fi.FullName) || !fi.Exists)
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

        SectionOption = new Option<uint?>(
            name: "--section",
            description: "Sections to look for when querying the blockchain.",
            isDefault: true, //Default value exists.
            parseArgument: result => // Validation.
            {
                if (!result.Tokens.Any())
                {
                    return null; // Default.
                }

                string sectionString = result.Tokens.SingleOrDefault()?.Value ?? string.Empty;
                if (!uint.TryParse(sectionString, out uint section))
                {
                    result.ErrorMessage = result.LocalizationResources.ArgumentConversionCannotParse("SectionNumber", typeof(uint));
                    return null;
                }

                return section;
            });
        SectionOption.AddValidator(result =>
        {
            if (result.Tokens.Count == 0)
            {
                result.ErrorMessage = "Must set a value for option --section, --candidate or both.";
            }
        });

        CandidateOption = new Option<uint?>(
            name: "--candidate",
            description: "Candidate to look for when querying the blockchain.",
            isDefault: true, //Default value exists.
            parseArgument: result => // Validation.
            {
                if (!result.Tokens.Any())
                {
                    return null; // Default.
                }

                string candidateString = result.Tokens.SingleOrDefault()?.Value ?? string.Empty;
                if (!uint.TryParse(candidateString, out uint candidate))
                {
                    result.ErrorMessage = result.LocalizationResources.ArgumentConversionCannotParse("CandidateNumber", typeof(uint));
                    return null;
                }

                return candidate; // Default
            });
        CandidateOption.AddValidator(result =>
        {
            if (result.Tokens.Count == 0)
            {
                result.ErrorMessage = "Must set a value for option --section, --candidate or both.";
            }
        });
    }
}