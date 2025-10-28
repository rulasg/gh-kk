using System;
using System.CommandLine;
using gh_kk.Interfaces;

namespace gh_kk;

public static class SubCommand2
{
    public static RootCommand AddSubCommand2(this RootCommand rootCommand, IGlobalOptions globalOptions)
    {
        ArgumentNullException.ThrowIfNull(rootCommand);
        ArgumentNullException.ThrowIfNull(globalOptions);

        var cmd2Arg1 = new Argument<string>(
            name: "owner",
            description: "The owner of the project."
        );

        var cmd2Arg2 = new Argument<int>(
            name: "number",
            description: "The number of the project."
        );

        var cmd2Opt1 = new Option<string>(
            name: "--description",
            description: "The description of the project. (Command)"
        );

        var subCommand2 = new Command(
            name: "subcommand2",
            description: "Call sub command 2"
        )
        {
            cmd2Arg1,
            cmd2Arg2,
            cmd2Opt1
        };

        var verboseOption = globalOptions.GetOption<bool>("verbose");

        subCommand2.SetHandler((owner, number, description, verbose) =>
        {
            Invoke(owner, number, description, verbose);
        }, cmd2Arg1, cmd2Arg2, cmd2Opt1, verboseOption);

        rootCommand.AddCommand(subCommand2);

        return rootCommand;
    }

    public static void Invoke(string owner, int number, string? description, bool verbose)
    {
        var message = $"Called SubCommand2 {number} owned by {owner}.";
        
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
