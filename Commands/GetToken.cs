using System;
using System.Collections;
using System.CommandLine;

namespace gh_kk.Commands;

public static class GetToken
{
    public static RootCommand AddGetTokenCommand(this RootCommand rootCommand)
    {
        var verboseOption = GlobalOptions.GetOption<bool>("verbose");

        var getTokenCommand = new Command(
            name: "get-token",
            description: "Get auth token from GitHub CLI."
        );

        getTokenCommand.SetHandler((verbose) =>
        {
            GetToken.Invoke(verbose);
        }, verboseOption);

        rootCommand.AddCommand(getTokenCommand);

        return rootCommand;
    }

    public static void Invoke(bool verbose)
    {
        var token = gh_kk.Integrations.gh_Integration.GetToken(verbose);
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
