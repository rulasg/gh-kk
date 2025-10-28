using System;
using gh_kk.Interfaces;

namespace gh_kk.Integration;

public sealed class GhIntegration : IGhIntegration
{
    private readonly IOsIntegration _osIntegration;

    public GhIntegration(IOsIntegration osIntegration)
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
                Console.Error.WriteLine($"Failed to get GitHub token. Error: {result.Error}");
                return string.Empty;
            }

            if (verbose)
            {
                Console.WriteLine("Successfully retrieved GitHub token.");
            }

            return result.Output.Trim();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred while getting the GitHub token: {ex.Message}");
            
            if (verbose)
            {
                Console.Error.WriteLine(ex.ToString());
            }

            return string.Empty;
        }
    }

    public string GetActiveUser(bool verbose)
    {
        try
        {
            var result = _osIntegration.RunConsoleProcess("gh", "api user", verbose);

            if (!result.Success)
            {
                Console.Error.WriteLine($"Failed to get active user information. Error: {result.Error}");
                return string.Empty;
            }

            if (verbose)
            {
                Console.WriteLine("Successfully retrieved active user information.");
            }

            return result.Output.Trim();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred while getting the active user: {ex.Message}");
            
            if (verbose)
            {
                Console.Error.WriteLine(ex.ToString());
            }

            return string.Empty;
        }
    }
}
