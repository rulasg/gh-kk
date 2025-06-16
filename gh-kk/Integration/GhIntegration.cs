using System;
using gh_kk.Interfaces;

namespace gh_kk.Integration;

public class GhIntegration : IGhIntegration
{
    private readonly IOsIntegration _osIntegration;

    public GhIntegration(IOsIntegration? osIntegration)
    {
        _osIntegration = osIntegration ?? throw new ArgumentNullException(nameof(osIntegration));
    }

    public string GetToken(bool verbose)
    {
        try
        {
            var result = _osIntegration.RunConsoleProcess("gh", "auth token", verbose);

            if (!result.Success)
            {
                throw new Exception($"Failed to get GitHub token. Error: {result.Error}");
            }

            if (verbose)
            {
                Console.WriteLine("Successfully retrieved GitHub token.");
            }

            return result.Output.Trim();
        }
        catch 
        {
            throw;
        }
    }
}
