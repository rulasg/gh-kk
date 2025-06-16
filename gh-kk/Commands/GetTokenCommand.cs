using System;
using System.Collections;
using System.CommandLine;
using gh_kk.Integration;
using gh_kk.Integration.Interfaces;

namespace gh_kk.Commands;

public static class GetTokenCommand
{
    public static RootCommand AddGetTokenCommand(this RootCommand rootCommand, IGhIntegration ghIntegration, GlobalOptions globalOptions)
    {
        var verboseOption = globalOptions.GetOption<bool>("verbose");

        var getTokenCommand = new Command(
            name: "get-token",
            description: "Get auth token from GitHub CLI."
        );

        getTokenCommand.SetHandler((verbose) =>
        {
            GetTokenCommand.Invoke(ghIntegration, verbose);
        }, verboseOption);

        rootCommand.AddCommand(getTokenCommand);

        return rootCommand;
    }

    public static void Invoke(IGhIntegration ghIntegration, bool verbose)
    {
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
