using System;
using System.IO;
using System.Text;
using Xunit;
using Moq;
using gh_kk.Integration.Interfaces;
using gh_kk.Commands;
using System.CommandLine;


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
    
    [Fact]
    public void Should_output_token_when_successful()
    {
        // Arrange
        var mockGhIntegration = new Mock<IGhIntegration>();
        mockGhIntegration.Setup(m => m.GetToken(false)).Returns("mock_token_value");
        
        // Redirect console output for assertion
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        // Act
        GetTokenCommand.Invoke(mockGhIntegration.Object, false);
        var result = consoleOutput.ToString().TrimEnd();
        
        // Cleanup
        Console.SetOut(Console.Out);
        
        // Assert
        Assert.Equal("mock_token_value", result);
        mockGhIntegration.Verify(m => m.GetToken(false), Times.Once);
    }
    
    [Fact]
    public void Should_output_token_when_successful_with_verbose()
    {
        // Arrange
        var mockGhIntegration = new Mock<IGhIntegration>();
        mockGhIntegration.Setup(m => m.GetToken(true)).Returns("mock_token_value");
        
        // Redirect console output for assertion
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        // Act
        GetTokenCommand.Invoke(mockGhIntegration.Object, true);
        var result = consoleOutput.ToString().TrimEnd();
        
        // Cleanup
        Console.SetOut(Console.Out);
        
        // Assert
        Assert.Equal("mock_token_value", result);
        mockGhIntegration.Verify(m => m.GetToken(true), Times.Once);
    }
    
    [Fact]
    public void Should_output_error_when_token_retrieval_fails()
    {
        // Arrange
        var mockGhIntegration = new Mock<IGhIntegration>();
        mockGhIntegration.Setup(m => m.GetToken(false)).Returns(string.Empty);
        
        // Redirect error output for assertion
        var originalError = Console.Error;
        var errorOutput = new StringWriter();
        Console.SetError(errorOutput);
        
        // Redirect standard output to avoid interference
        var originalOut = Console.Out;
        Console.SetOut(new StringWriter());
        
        // Act
        GetTokenCommand.Invoke(mockGhIntegration.Object, false);
        var result = errorOutput.ToString().TrimEnd();
        
        // Cleanup
        Console.SetError(originalError);
        Console.SetOut(originalOut);
        
        // Assert
        Assert.Equal("Failed to retrieve GitHub token.", result);
    }
    
    [Fact]
    public void Should_add_command_to_root_command()
    {
        // Arrange
        var rootCommand = new RootCommand();
        var mockGhIntegration = new Mock<IGhIntegration>();
        var globalOptions = new gh_kk.GlobalOptions();
        globalOptions.AddOption("verbose", new Option<bool>("--verbose"));
        
        // Act
        var result = GetTokenCommand.AddGetTokenCommand(rootCommand, mockGhIntegration.Object, globalOptions);
        
        // Assert
        Assert.Same(rootCommand, result);
        Assert.Single(rootCommand.Subcommands, cmd => cmd.Name == "get-token");
    }
    
    [Fact]
    public void Should_execute_command_when_invoked_from_commandline()
    {
        // Arrange
        var cmdTest = new CmdTest();
        var args = new string[] { "get-token" };
        
        // Act
        var result = cmdTest.RunAndGetConsoleOutput(args);
        
        // Assert
        // Note: This test will depend on the actual state of gh auth.
        // If not authenticated, there should be an error message
        // If authenticated, there should be a token
        // We're just checking that the command is executed
        Assert.NotEmpty(result);
    }
    
    [Fact]
    public void Should_execute_command_with_verbose_flag()
    {
        // Arrange
        var cmdTest = new CmdTest();
        var args = new string[] { "get-token", "--verbose" };
        
        // Act
        var result = cmdTest.RunAndGetConsoleOutput(args);
        
        // Assert
        // Note: This test will depend on the actual state of gh auth
        Assert.NotEmpty(result);
    }
}