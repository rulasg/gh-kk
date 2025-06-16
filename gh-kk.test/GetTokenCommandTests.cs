using System;
using System.IO;
using System.Text;
using Xunit;


namespace gh_kk.test;



public class GetTokenCommandTests
{
    [Fact]
    public void Should_returnversion_when_versionargumentisprovided()
    {
        // Arrange
        var cmdTest = new CmdTest();
        var args = new string[] { "--version" };

        // Act
        var result = cmdTest.RunAndGetConsoleOutput(args);

        // Assert
        Assert.Contains("17.11.1", result);
    }

    [Fact]
    public void Should_returnhelp_when_no_arguments()
    {
        // Arrange
        // var args = new string[] { "" };
        var cmdTest = new CmdTest();
        string[]? args = null;

        // Act
        var result = cmdTest.RunAndGetConsoleOutput(args);

        // Output result to test console
        System.Console.WriteLine("=== Command Output ===");
        System.Console.WriteLine(result);
        System.Console.WriteLine("====================");

        // Assert
        Assert.Contains("Description:", result);
        Assert.Contains("  Sample app for System.CommandLine", result);
        Assert.Contains("Usage:", result);
        Assert.Contains("  testhost [command] [options]", result);
        Assert.Contains("Options:", result);
        Assert.Contains("  -v, --verbose   Enable verbose output (global)", result);
        Assert.Contains("  --version       Show version information", result);
        Assert.Contains("  -?, -h, --help  Show help and usage information", result);
        Assert.Contains("Commands:", result);
        Assert.Contains("  subcommand1 <owner> <number>  Call sub command 1", result);
        Assert.Contains("  subcommand2 <owner> <number>  Call sub command 2", result);
        Assert.Contains("  get-token                     Get auth token from GitHub CLI.", result);
    }
    
}