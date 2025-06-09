/*
Sample app for System.CommandLine
This is a simple console application that demonstrates how to use System.CommandLine
to create a command-line interface (CLI) with a commands to manage projects.

REFERENCE: https://learn.microsoft.com/en-us/dotnet/standard/commandline/
*/

using System.Collections;
using System.CommandLine;
using System.Net.NetworkInformation;

namespace gh_kk;

class Program
{

    static Hashtable globalOptions = new Hashtable();

    static async Task<int> Main(string[] args)
    {

        var rootCommand = new RootCommand("Sample app for System.CommandLine"); // No action if no parameter is provided

        var verboseOption = new Option<bool>(
            aliases: new[] { "--verbose", "-v" },
            description: "Enable verbose output (global)");
        verboseOption.AddAlias("-v");

        rootCommand.AddGlobalOption(verboseOption);
        GlobalOptions.AddOption("verbose", verboseOption);

        rootCommand.AddSubCommand1().AddSubCommand2();

        if (args.Length == 0)
            return await rootCommand.InvokeAsync("--help");

        return await rootCommand.InvokeAsync(args);
    }

}

static class GlobalOptions
{
    static Hashtable options;

    static GlobalOptions()
    {
        options = new Hashtable();
    }

    public static void AddOption(string name, Option option)
    {
        if (!options.ContainsKey(name))
        {
            options.Add(name, option);
        }
    }

    public static Option<T> GetOption<T>(string name)
    {
        Option<T>? optionValue = options[name] as Option<T>;

        if (optionValue == null)
        {
            throw new ArgumentException($"Option {name} is not of type Option<{typeof(T).Name}>");
        }

        return optionValue;
    }
}
