﻿using System.CommandLine;

namespace example;

public class Example
{
    static async Task<int> ExampleMain(string[] args)
    {
        //Options(flags)
        var fileOption = new Option<FileInfo?>(
            name: "--file",
            description: "An option whose argument is parsed as a FileInfo",
            isDefault: true, //Diz que existe um default.
            parseArgument: result => // Validação.
            {
                if (result.Tokens.Count == 0)
                {
                    return new FileInfo("sampleQuotes.txt");
                }

                string? filePath = result.Tokens.Single().Value;
                if (!File.Exists(filePath))
                {
                    result.ErrorMessage = "File does not exist";
                    return null;
                }
                else
                {
                    return new FileInfo(filePath); // Default
                }
            });
        
        

        var searchTermsOption = new Option<string[]>(
            name: "--search-terms",
            description: "Strings to search for when deleting entries.")
        {
            //Settings
            IsRequired = true,
            AllowMultipleArgumentsPerToken = true
        };


        //Argumentos. Também é possível configurá-los separadamente.
        var quoteArgument = new Argument<string>(
            name: "quote",
            description: "Text of quote.");

        var bylineArgument = new Argument<string>(
            name: "byline",
            description: "Byline of quote.");

        var delayOption = new Option<int>(
            name: "--delay",
            description: "Delay between lines, specified as milliseconds per character in a line.",
            getDefaultValue: () => 42);

        var fgcolorOption = new Option<ConsoleColor>(
            name: "--fgcolor",
            description: "Foreground color of text displayed on the console.",
            getDefaultValue: () => ConsoleColor.White);

        var lightModeOption = new Option<bool>(
            name: "--light-mode",
            description: "Background color of text displayed on the console: default is black, light mode is white.");


        //Cria comandos.

        //Root
        var rootCommand = new RootCommand("Sample app for System.CommandLine");
        // Global options are applied to the command and recursively to subcommands. 
        // Since --file is on the root command, it will be available automatically in all subcommands of the app.
        rootCommand.AddGlobalOption(fileOption);

        //Sub1
        var quotesCommand = new Command("quotes", "Work with a file that contains quotes.");
        rootCommand.AddCommand(quotesCommand);

        //Sub1Sub1
        var readCommand = new Command("read", "Read and display the file.")
        {
            delayOption,
            fgcolorOption,
            lightModeOption
        };
        quotesCommand.AddCommand(readCommand);

        //Sub1Sub2
        var deleteCommand = new Command("delete", "Delete lines from the file.");
        deleteCommand.AddOption(searchTermsOption); //Adiciona options.
        quotesCommand.AddCommand(deleteCommand);

        //Sub1Sub3
        var addCommand = new Command("add", "Add an entry to the file.");
        addCommand.AddArgument(quoteArgument); // Adicina argumentos
        addCommand.AddArgument(bylineArgument);
        addCommand.AddAlias("insert"); //Alias para commando.
        quotesCommand.AddCommand(addCommand); //Adiciona a Sub1.


        //Handlers.
        //read
        readCommand.SetHandler(
            async (file, delay, fgcolor, lightMode) => { await ReadFile(file!, delay, fgcolor, lightMode); },
            fileOption, delayOption, fgcolorOption, lightModeOption);

        //delete
        deleteCommand.SetHandler((file, searchTerms) => { DeleteFromFile(file!, searchTerms); },
            fileOption, searchTermsOption);

        //add
        addCommand.SetHandler((file, quote, byline) => { AddToFile(file!, quote, byline); },
            fileOption, quoteArgument, bylineArgument);

        //Ação do Comando root.
        // rootCommand.SetHandler((file) => 
        // { 
        //     ReadFile(file!); 
        // },
        // fileOption);

        return await rootCommand.InvokeAsync(args);
    }

    //Métodos a serem invocados.
    internal static async Task ReadFile(
        FileInfo file, int delay, ConsoleColor fgColor, bool lightMode)
    {
        Console.BackgroundColor = lightMode ? ConsoleColor.White : ConsoleColor.Black;
        Console.ForegroundColor = fgColor;
        List<string> lines = File.ReadLines(file.FullName).ToList();
        foreach (string line in lines)
        {
            Console.WriteLine(line);
            await Task.Delay(delay * line.Length);
        }

        ;
    }

    internal static void DeleteFromFile(FileInfo file, string[] searchTerms)
    {
        Console.WriteLine("Deleting from file");
        File.WriteAllLines(
            file.FullName, File.ReadLines(file.FullName)
                .Where(line => searchTerms.All(s => !line.Contains(s))).ToList());
    }

    internal static void AddToFile(FileInfo file, string quote, string byline)
    {
        Console.WriteLine("Adding to file");
        using StreamWriter? writer = file.AppendText();
        writer.WriteLine($"{Environment.NewLine}{Environment.NewLine}{quote}");
        writer.WriteLine($"{Environment.NewLine}-{byline}");
        writer.Flush();
    }
}