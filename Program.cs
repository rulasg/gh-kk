/*
Sample app for System.CommandLine
This is a simple console application that demonstrates how to use System.CommandLine
to create a command-line interface (CLI) with a commands to manage projects.

REFERENCE: https://learn.microsoft.com/en-us/dotnet/standard/commandline/
*/

using System.CommandLine;

namespace kk;

class Program
{
    static async Task<int> Main(string[] args)
    {

        var rootCommand = new RootCommand("Sample app for System.CommandLine"); // No action if no parameter is provided

        SetupSubcommand1(rootCommand);

        return await rootCommand.InvokeAsync(args);
    }



    // Function to setup the project get command
    static void SetupSubcommand1(RootCommand rootCommand)
    {
        var subCommandOption1 = new Option<string>(
            name: "--owner",
            description: "The owner of the project."
        );

        var subCommandOption2 = new Option<int>(
            name: "--number",
            description: "The number of the project."
        );

        var subCommand1 = new Command(
            name: "subcommand1",
            description: "Call sub command 1"
        ){
                subCommandOption1,
                subCommandOption2
            };


        subCommand1.SetHandler((opt1, opt2) =>
        {
            var str = $"Called SubCommand1 {opt2} owned by {opt1}.";
            Console.WriteLine(str);

        }, subCommandOption1, subCommandOption2);

        rootCommand.AddCommand(subCommand1);

    }
}
