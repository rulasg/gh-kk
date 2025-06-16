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
namespace gh_kk
{

    internal class Program
    {

        public static async Task<int> Main(string[]? args)
        {

            var rootCommand = new RootCommand("Sample app for System.CommandLine"); // No action if no parameter is provided

            var verboseOption = new Option<bool>(
                aliases: new[] { "--verbose", "-v" },
                description: "Enable verbose output (global)");
            verboseOption.AddAlias("-v");

            rootCommand.AddGlobalOption(verboseOption);
            IGlobalOptions globalOptions = new GlobalOptions();
            globalOptions.AddOption("verbose", verboseOption);

            // Set up dependencies
            var osIntegration = new Integration.OsIntegration();
            var ghIntegration = new Integration.GhIntegration(osIntegration);

            rootCommand
                .AddSubCommand1(globalOptions)
                .AddSubCommand2(globalOptions)
                .AddGetTokenCommand(ghIntegration, globalOptions);

            if (args == null)
                return await rootCommand.InvokeAsync("--help");

            return await rootCommand.InvokeAsync(args);
        }

    }

    public class MyCustomBinder : BinderBase<IOsIntegration>
    {
        protected override IOsIntegration GetBoundValue(System.CommandLine.Binding.BindingContext bindingContext)
            => new OsIntegration();
    }

}
