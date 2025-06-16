using System;

namespace gh_kk.Integration;

public static class GhIntegration
{
    public static string GetToken(bool verbose)
    {
        try
        {
            var result = OsIntegration.RunConsoleProcess("gh", "auth token", verbose);

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
}
