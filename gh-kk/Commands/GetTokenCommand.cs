using System;
using System.Collections;
using System.CommandLine;
using gh_kk.Integration;
using gh_kk.Interfaces;

namespace gh_kk.Commands;

public static class GetTokenCommand
{
    public static RootCommand AddGetTokenCommand(this RootCommand rootCommand, OsIntegrationBinder customBinder, IGlobalOptions globalOptions)
    {
        var verboseOption = globalOptions.GetOption<bool>("verbose");

        var getTokenCommand = new Command(
            name: "get-token",
            description: "Get auth token from GitHub CLI."
        );

        getTokenCommand.SetHandler((osIntegration,verbose) =>
        {

            var ghIntegration = new GhIntegration(osIntegration);

            GetTokenCommand.Invoke(ghIntegration, verbose);

        }, customBinder,verboseOption );

        rootCommand.AddCommand(getTokenCommand);

        return rootCommand;
    }

    public static void Invoke(IGhIntegration ghIntegration, bool verbose)
    {
        try
        {
            var output = ghIntegration.GetToken(verbose);
            Console.WriteLine(output);
        }
        catch 
        {
            Console.Error.WriteLine("Failed to retrieve GitHub token.");
        }
    }
}
