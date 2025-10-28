/*
Sample app for System.CommandLine
This is a simple console application that demonstrates how to use System.CommandLine
to create a command-line interface (CLI) with a commands to manage projects.

REFERENCE: https://learn.microsoft.com/en-us/dotnet/standard/commandline/
*/

using System.CommandLine;
using System.Runtime.CompilerServices;
using gh_kk.Commands;
using gh_kk.Integration;
using gh_kk.Interfaces;

[assembly: InternalsVisibleTo("gh-kk.test")]
namespace gh_kk;

internal static class Program
{
    public static async Task<int> Main(string[]? args)
    {
        var rootCommand = new RootCommand("Sample app for System.CommandLine");

        var verboseOption = new Option<bool>(
            aliases: ["--verbose", "-v"],
            description: "Enable verbose output (global)");

        rootCommand.AddGlobalOption(verboseOption);
        IGlobalOptions globalOptions = new GlobalOptions();
        globalOptions.AddOption("verbose", verboseOption);

        var osIntegration = new OsIntegration();
        var ghIntegration = new GhIntegration(osIntegration);

        rootCommand
            .AddSubCommand1(globalOptions)
            .AddSubCommand2(globalOptions)
            .AddGetTokenCommand(ghIntegration, globalOptions);

        if (args is null)
        {
            return await rootCommand.InvokeAsync("--help").ConfigureAwait(false);
        }

        return await rootCommand.InvokeAsync(args).ConfigureAwait(false);
    }
}
