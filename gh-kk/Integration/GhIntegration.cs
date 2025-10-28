using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using gh_kk.Interfaces;

namespace gh_kk.Integration;

public sealed class GhIntegration : IGhIntegration
{
    private readonly IOsIntegration _osIntegration;
    private static readonly HttpClient _httpClient = new();

    public GhIntegration(IOsIntegration osIntegration)
    {
        _osIntegration = osIntegration ?? throw new ArgumentNullException(nameof(osIntegration));
    }

    public string GetToken(bool verbose)
    {
        try
        {
            // Check if GH_HOST is set to get token for specific host
            var ghHost = Environment.GetEnvironmentVariable("GH_HOST");
            var args = string.IsNullOrEmpty(ghHost) ? "auth token" : $"auth token --hostname {ghHost}";
            
            var result = _osIntegration.RunConsoleProcess("gh", args, verbose);

            if (!result.Success)
            {
                Console.Error.WriteLine($"Failed to get GitHub token. Error: {result.Error}");
                return string.Empty;
            }

            if (verbose)
            {
                var hostMessage = string.IsNullOrEmpty(ghHost) ? "default" : ghHost;
                Console.WriteLine($"Successfully retrieved GitHub token for {hostMessage}.");
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

    public string GetHostname(bool verbose)
    {
        try
        {
            // First check if GH_HOST environment variable is set
            var ghHost = Environment.GetEnvironmentVariable("GH_HOST");
            if (!string.IsNullOrEmpty(ghHost))
            {
                if (verbose)
                {
                    Console.WriteLine($"Using hostname from GH_HOST environment variable: {ghHost}");
                }
                return ghHost;
            }

            var result = _osIntegration.RunConsoleProcess("gh", "auth status", verbose);

            // gh auth status writes to stderr, not stdout
            var output = !string.IsNullOrEmpty(result.Output) ? result.Output : result.Error;

            if (string.IsNullOrEmpty(output))
            {
                if (verbose)
                {
                    Console.WriteLine("No auth status output, defaulting to github.com");
                }
                return "github.com";
            }

            // Get the default token to compare
            var tokenResult = _osIntegration.RunConsoleProcess("gh", "auth token", false);
            var defaultToken = tokenResult.Success ? tokenResult.Output.Trim() : string.Empty;

            if (string.IsNullOrEmpty(defaultToken))
            {
                if (verbose)
                {
                    Console.WriteLine("Could not get default token, defaulting to github.com");
                }
                return "github.com";
            }

            // Parse the output to find which host has this token
            var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            
            string? currentHost = null;

            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                
                // Check if this is a hostname line (no leading spaces and not starting with special chars)
                if (!line.StartsWith(" ") && !line.StartsWith("✓") && !line.StartsWith("-"))
                {
                    currentHost = trimmed;
                }
                // Check for token in the line (it will be masked with asterisks)
                else if (trimmed.StartsWith("- Token:") && currentHost != null)
                {
                    // Get the token for this specific host to verify
                    var hostTokenResult = _osIntegration.RunConsoleProcess("gh", $"auth token --hostname {currentHost}", false);
                    if (hostTokenResult.Success && hostTokenResult.Output.Trim() == defaultToken)
                    {
                        if (verbose)
                        {
                            Console.WriteLine($"Found active GitHub instance: {currentHost}");
                        }
                        return currentHost;
                    }
                }
            }

            // If no match found, default to github.com
            if (verbose)
            {
                Console.WriteLine("No matching hostname found for default token, defaulting to github.com");
            }
            return "github.com";
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"An error occurred while getting the GitHub hostname: {ex.Message}");
            
            if (verbose)
            {
                Console.Error.WriteLine(ex.ToString());
            }

            return "github.com";
        }
    }

    public string GetActiveUser(bool verbose)
    {
        try
        {
            var token = GetToken(verbose);
            
            if (string.IsNullOrEmpty(token))
            {
                Console.Error.WriteLine("Unable to retrieve token for API authentication.");
                return string.Empty;
            }

            var hostname = GetHostname(verbose);
            var userJson = GetActiveUserAsync(hostname, token, verbose).GetAwaiter().GetResult();
            return userJson;
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

    private static async Task<string> GetActiveUserAsync(string hostname, string token, bool verbose)
    {
        try
        {
            var apiUrl = $"https://{hostname}/api/v3/user";
            
            // For github.com, use the standard API endpoint
            if (hostname.Equals("github.com", StringComparison.OrdinalIgnoreCase))
            {
                apiUrl = "https://api.github.com/user";
            }

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("gh-kk", "1.0"));
            request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");

            if (verbose)
            {
                Console.WriteLine($"Calling GitHub API: GET {apiUrl}");
            }

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                Console.Error.WriteLine($"GitHub API request failed with status {response.StatusCode}");
                
                if (verbose)
                {
                    Console.Error.WriteLine($"Error response: {errorContent}");
                }
                
                return string.Empty;
            }

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (verbose)
            {
                Console.WriteLine("Successfully retrieved active user information from GitHub API.");
            }

            return content;
        }
        catch (HttpRequestException ex)
        {
            Console.Error.WriteLine($"HTTP request failed: {ex.Message}");
            return string.Empty;
        }
    }
}
