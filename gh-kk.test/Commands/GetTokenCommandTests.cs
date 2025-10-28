using System;
using System.IO;
using System.CommandLine;
using Moq;
using gh_kk.Interfaces;
using gh_kk.Commands;

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
        Assert.Contains("  active-user                   Get the active GitHub user information.", result);
    }
    
    [Fact]
    public void Should_output_user_info_when_successful()
    {
        // Arrange
        var mockGhIntegration = new Mock<IGhIntegration>();
        var mockUserJson = @"{
            ""login"": ""testuser"",
            ""name"": ""Test User"",
            ""email"": ""test@example.com"",
            ""company"": ""Test Company"",
            ""bio"": ""A test user""
        }";
        mockGhIntegration.Setup(m => m.GetActiveUser(false)).Returns(mockUserJson);
        
        // Redirect console output for assertion
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        // Act
        GetTokenCommand.Invoke(mockGhIntegration.Object, false);
        var result = consoleOutput.ToString();
        
        // Cleanup
        Console.SetOut(Console.Out);
        
        // Assert
        Assert.Contains("Active GitHub User: testuser", result);
        Assert.Contains("Name: Test User", result);
        Assert.Contains("Email: test@example.com", result);
        Assert.Contains("Company: Test Company", result);
        Assert.Contains("Bio: A test user", result);
        mockGhIntegration.Verify(m => m.GetActiveUser(false), Times.Once);
    }
    
    [Fact]
    public void Should_output_user_info_with_verbose()
    {
        // Arrange
        var mockGhIntegration = new Mock<IGhIntegration>();
        var mockUserJson = @"{
            ""login"": ""testuser"",
            ""name"": ""Test User""
        }";
        mockGhIntegration.Setup(m => m.GetActiveUser(true)).Returns(mockUserJson);
        
        // Redirect console output for assertion
        var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        
        // Act
        GetTokenCommand.Invoke(mockGhIntegration.Object, true);
        var result = consoleOutput.ToString();
        
        // Cleanup
        Console.SetOut(Console.Out);
        
        // Assert
        Assert.Contains("Active GitHub User: testuser", result);
        Assert.Contains("Name: Test User", result);
        Assert.Contains("Full JSON Response:", result);
        mockGhIntegration.Verify(m => m.GetActiveUser(true), Times.Once);
    }
    
    [Fact]
    public void Should_output_error_when_user_retrieval_fails()
    {
        // Arrange
        var mockGhIntegration = new Mock<IGhIntegration>();
        mockGhIntegration.Setup(m => m.GetActiveUser(false)).Returns(string.Empty);
        
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
        Assert.Equal("Failed to retrieve active user information.", result);
    }
    
    [Fact]
    public void Should_add_command_to_root_command()
    {
        // Arrange
        var rootCommand = new RootCommand();
        var mockGhIntegration = new Mock<IGhIntegration>();
        var globalOptions = new GlobalOptions();
        globalOptions.AddOption("verbose", new Option<bool>("--verbose"));
        
        // Act
        var result = GetTokenCommand.AddGetTokenCommand(rootCommand, mockGhIntegration.Object, globalOptions);
        
        // Assert
        Assert.Same(rootCommand, result);
        Assert.Single(rootCommand.Subcommands, cmd => cmd.Name == "active-user");
    }

    [Fact]
    public void Should_execute_command_when_invoked_from_commandline()
    {
        // Arrange
        var cmdTest = new CmdTest();
        var args = new string[] { "active-user" };

        // Act
        var result = cmdTest.RunAndGetConsoleOutput(args);

        // Assert - should contain "Active GitHub User:" in output
        Assert.Contains(result, line => line.Contains("Active GitHub User:"));
    }

    [Fact]
    public void Should_execute_command_with_verbose_flag()
    {
        // Arrange
        var cmdTest = new CmdTest();
        var args = new string[] { "active-user", "--verbose" };

        // Act
        var result = cmdTest.RunAndGetConsoleOutput(args);

        // Assert - should contain user info and verbose messages
        Assert.Contains(result, line => line.Contains("Active GitHub User:") || line.Contains("Successfully retrieved"));
    }
}