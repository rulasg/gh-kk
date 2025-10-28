using System;
using System.CommandLine;
using System.Text.Json;
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

        var userJson = ghIntegration.GetActiveUser(verbose);
        
        if (string.IsNullOrEmpty(userJson))
        {
            Console.Error.WriteLine("Failed to retrieve active user information.");
            return;
        }

        try
        {
            var jsonDoc = JsonDocument.Parse(userJson);
            var root = jsonDoc.RootElement;

            var login = root.GetProperty("login").GetString();
            var name = root.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : null;
            var email = root.TryGetProperty("email", out var emailElement) ? emailElement.GetString() : null;
            var company = root.TryGetProperty("company", out var companyElement) ? companyElement.GetString() : null;
            var bio = root.TryGetProperty("bio", out var bioElement) ? bioElement.GetString() : null;

            Console.WriteLine($"Active GitHub User: {login}");
            
            if (!string.IsNullOrEmpty(name))
            {
                Console.WriteLine($"Name: {name}");
            }
            
            if (!string.IsNullOrEmpty(email))
            {
                Console.WriteLine($"Email: {email}");
            }
            
            if (!string.IsNullOrEmpty(company))
            {
                Console.WriteLine($"Company: {company}");
            }
            
            if (!string.IsNullOrEmpty(bio))
            {
                Console.WriteLine($"Bio: {bio}");
            }

            if (verbose)
            {
                Console.WriteLine("\nFull JSON Response:");
                Console.WriteLine(JsonSerializer.Serialize(root, new JsonSerializerOptions { WriteIndented = true }));
            }
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Failed to parse user information: {ex.Message}");
            
            if (verbose)
            {
                Console.Error.WriteLine($"Raw response: {userJson}");
            }
        }
    }
}
