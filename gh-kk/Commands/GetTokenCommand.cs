using System;
using System.CommandLine;
using gh_kk.Interfaces;

namespace gh_kk.Commands;

public static class GetTokenCommand
{
    public static RootCommand AddGetTokenCommand(this RootCommand rootCommand, IGhIntegration ghIntegration, IGlobalOptions globalOptions)
    {
        ArgumentNullException.ThrowIfNull(rootCommand);
        ArgumentNullException.ThrowIfNull(ghIntegration);
        ArgumentNullException.ThrowIfNull(globalOptions);

        var verboseOption = globalOptions.GetOption<bool>("verbose");

        var getTokenCommand = new Command(
            name: "active-user",
            description: "Get the active GitHub user information."
        );

        getTokenCommand.SetHandler((verbose) =>
        {
            Invoke(ghIntegration, verbose);
        }, verboseOption);

        rootCommand.AddCommand(getTokenCommand);

        return rootCommand;
    }

    public static void Invoke(IGhIntegration ghIntegration, bool verbose)
    {
        ArgumentNullException.ThrowIfNull(ghIntegration);

        var token = ghIntegration.GetToken(verbose);
        
        if (!string.IsNullOrEmpty(token))
        {
            Console.WriteLine(token);
        }
        else
        {
            Console.Error.WriteLine("Failed to retrieve GitHub token.");
        }
    }
}
