using System.IO;
using System.Text;
using Xunit;


namespace gh_kk.test;

public class GetTokenCommandTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var args = new string[] { "--version" };

        // Act
        var result = cmdTest.RunAndGetConsoleOutput(args);

        // Assert
        Assert.Contains("17.11.1", result);
    }

    [Fact]
    public void Test_NoParameters()
    {
        // Arrange
        // var args = new string[] { "" };
        string[]? args = null;

        // Act
        var result = cmdTest.RunAndGetConsoleOutput(args);

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

    [Fact]
    public void Test_GetToken()
    {
        // Arrange
        var expectedToken = gh_kk.Integration.OsIntegration.RunConsoleProcess("gh", "auth token").Output.Trim();

        var args = new string[] { "get-token" };

        // Act
        var result = cmdTest.RunAndGetConsoleOutput(args);

        var tokenPrompt = result[0];
        
        // Assert
        Assert.Contains(expectedToken, result);
    }

}