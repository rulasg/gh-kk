/*
Sample app for System.CommandLine
This is a simple console application that demonstrates how to use System.CommandLine
to create a command-line interface (CLI) with a commands to manage projects.

REFERENCE: https://learn.microsoft.com/en-us/dotnet/standard/commandline/
*/

using System.Collections;
using System.CommandLine;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.CommandLine.Binding;
using gh_kk.Commands;
using gh_kk.Integration;
using gh_kk.Interfaces;

[assembly: InternalsVisibleTo("gh-kk.test")]
namespace gh_kk;

internal class Program
{

    public static async Task<int> Main(string[]? args)
    {

        var rootCommand = new RootCommand("Sample app for System.CommandLine"); // No action if no parameter is provided

        // Define a global option for verbose output
        var verboseOption = new Option<bool>(aliases: ["--verbose", "-v"], description: "Enable verbose output (global)");
        rootCommand.AddGlobalOption(verboseOption);

        // Create global options instance to use as parameters for commands constructors
        IGlobalOptions globalOptions = new GlobalOptions();
        globalOptions.AddOption("verbose", verboseOption);


        // Set up dependencies
        var ghIntegration = new GhIntegration(new OsIntegration());

        var customBinder = new OsIntegrationBinder(new OsIntegration());

        rootCommand
            .AddSubCommand1(globalOptions)
            .AddSubCommand2(globalOptions)
            .AddGetTokenCommand(customBinder, globalOptions);

        if (args == null)
            return await rootCommand.InvokeAsync("--help");

        return await rootCommand.InvokeAsync(args);
    }

}

public class OsIntegrationBinder : BinderBase<IOsIntegration>
{
    private readonly IOsIntegration _osIntegration;

    // constructor
    public OsIntegrationBinder(IOsIntegration osIntegration)
    {
        _osIntegration = osIntegration;
    }
    
    protected override IOsIntegration GetBoundValue(BindingContext bindingContext)
        => _osIntegration;

}

