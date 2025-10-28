using System;
using System.CommandLine;
using gh_kk.Interfaces;

namespace gh_kk;

public static class SubCommand1
{
    public static RootCommand AddSubCommand1(this RootCommand rootCommand, IGlobalOptions globalOptions)
    {
        ArgumentNullException.ThrowIfNull(rootCommand);
        ArgumentNullException.ThrowIfNull(globalOptions);

        var cmd1Arg1 = new Argument<string>(
            name: "owner",
            description: "The owner of the project."
        );

        var cmd1Arg2 = new Argument<int>(
            name: "number",
            description: "The number of the project."
        );

        var cmd1Opt1 = new Option<string>(
            name: "--description",
            description: "The description of the project. (Command)"
        );

        var subCommand1 = new Command(
            name: "subcommand1",
            description: "Call sub command 1"
        )
        {
            cmd1Arg1,
            cmd1Arg2,
            cmd1Opt1
        };

        var verboseOption = globalOptions.GetOption<bool>("verbose");

        subCommand1.SetHandler((owner, number, description, verbose) =>
        {
            Invoke(owner, number, description, verbose);
        }, cmd1Arg1, cmd1Arg2, cmd1Opt1, verboseOption);

        rootCommand.AddCommand(subCommand1);
        
        return rootCommand;
    }

    public static void Invoke(string owner, int number, string? description, bool verbose)
    {
        var message = $"Called SubCommand1 {number} owned by {owner}.";
        
        if (!string.IsNullOrEmpty(description))
        {
            message += $"\nDescription: {description}";
        }
        
        if (verbose)
        {
            message += "\nVerbose output enabled.";
        }
        
        Console.WriteLine(message);
    }
}
