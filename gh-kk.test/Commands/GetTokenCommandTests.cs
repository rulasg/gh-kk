using System;
using System.IO;
using System.Text;
using Xunit;
using Moq;
using gh_kk.Interfaces;
using gh_kk.Commands;
using System.CommandLine;
using System.CommandLine.Binding;
using gh_kk.Integration;


namespace gh_kk.test;



public class GetTokenCommandTests
{
    [Fact]
    public void Should_output_token_when_successful()
    {
        // Arrange
        var mockOsIntegration = new Mock<IOsIntegration>();
        mockOsIntegration.Setup(m => m.RunConsoleProcess("gh", "auth token", false))
            .Returns(new Result("mock_token_value", "", 0));

        var mockGhIntegration = new Mock<IGhIntegration>();
        mockGhIntegration.Setup(m => m.GetToken(false)).Returns("mock_token_value");
        
        var rootCommand = new RootCommand();
        var globalOptions = new gh_kk.GlobalOptions();
        globalOptions.AddOption("verbose", new Option<bool>("--verbose"));
        
        var osIntegrationBinder = new OsIntegrationBinder(mockOsIntegration.Object);
        GetTokenCommand.AddGetTokenCommand(rootCommand, osIntegrationBinder, globalOptions);

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

        //mock IOsIntegration
        var mockOsIntegration = new Mock<IOsIntegration>();
        mockGhIntegration.Setup(m => m.GetToken(It.IsAny<bool>())).Returns("mock_token_value");
        var osIntegrationBinder = new OsIntegrationBinder(mockOsIntegration.Object);

        // Act
        var result = GetTokenCommand.AddGetTokenCommand(rootCommand, osIntegrationBinder, globalOptions);

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
        Environment.SetEnvironmentVariable("GH_TOKEN", "fakeToken");

        // Act
        var result = cmdTest.RunAndGetConsoleOutput(args);

        // Assert
        // Count occurrences of newline to determine number of lines in output
        Assert.Single(result, "fakeToken");

        // Clean up the environment variable after the test
        Environment.SetEnvironmentVariable("GH_TOKEN", null);
    }

    [Fact]
    public void Should_execute_command_with_verbose_flag()
    {
        // Arrange
        var cmdTest = new CmdTest();
        var args = new string[] { "get-token", "--verbose" };
        // Set the environment variable before running the test
        Environment.SetEnvironmentVariable("GH_TOKEN", "fakeToken");


        // Act
        var result = cmdTest.RunAndGetConsoleOutput(args);

        // Assert
        // Count occurrences of newline to determine number of lines in output
        Assert.Equal(2, result.Length);
        Assert.Contains("Successfully retrieved GitHub token.", result);
        Assert.Contains("fakeToken", result);

        // Clean up the environment variable after the test
        Environment.SetEnvironmentVariable("GH_TOKEN", null);
    }

    [Fact]
    public void Should_output_error_when_token_retrieval_fails_from_commandline()
    {
        // Arrange
        //mock IOsIntegration
        var mockOsIntegration = new Mock<IOsIntegration>();
        //return null when calling "gh auth token"
        mockOsIntegration.Setup(o => o.RunConsoleProcess("gh", "auth token", false))
            .Returns(new Result(string.Empty, "Failed to retrieve GitHub token.", 1));

        var ghIntegration = new GhIntegration(mockOsIntegration.Object);

        var result = ghIntegration.GetToken(false);

        Assert.Single(result, "Failed to retrieve GitHub token.");
        


    }
}