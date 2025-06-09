using System;

namespace gh_kk.Integrations;

public static class gh_Integration
{
    public static string GetToken(bool verbose)
    {
        try
        {
            var result = os_Integration.RunConsoleProcess("gh", "auth token", verbose);

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
