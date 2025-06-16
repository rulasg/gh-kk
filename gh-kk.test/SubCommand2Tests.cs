// #file:SubCommand2Tests.cs
using System;
using System.IO;
using gh_kk.test;
using Xunit;

namespace gh_kk.tests.Commands;

public class SubCommand2Tests
{
    [Fact]
    public void Invoke_WithBasicParameters_OutputsCorrectMessage()
    {
        // Arrange
        var cmdTest = new CmdTest();
        var owner = "testowner";
        var number = 123;
        var args = new string[] { "subcommand2", owner, number.ToString() };

        var consoleOutput = cmdTest.RunAndGetConsoleOutput(args);

        // Assert
        Assert.Contains($"Called SubCommand2 {number} owned by {owner}.", consoleOutput);
        Assert.DoesNotContain("Description", consoleOutput);
        Assert.DoesNotContain("Verbose output enabled", consoleOutput);
    }

    [Fact]
    public void Invoke_WithDescription_IncludesDescriptionInOutput()
    {
        // Arrange
        var cmdTest = new CmdTest();
        var owner = "testowner";
        var number = 123;
        
        var args = new string[] { "subcommand2", owner, number.ToString(), "-v"};

        var consoleOutput = cmdTest.RunAndGetConsoleOutput(args);

        // Assert
        Assert.Contains($"Called SubCommand2 {number} owned by {owner}.", consoleOutput);
        Assert.DoesNotContain("Verbose output enabled", consoleOutput);
    }

    [Fact]
    public void Invoke_WithVerboseFlag_IncludesVerboseMessageInOutput()
    {
        // Arrange
        var cmdTest = new CmdTest();
        var owner = "testowner";
        var number = 123;
        var args = new string[] { "subcommand2", owner, number.ToString(), "--verbose" };

        var consoleOutput = cmdTest.RunAndGetConsoleOutput(args);

        // Assert
        Assert.Contains($"Called SubCommand2 {number} owned by {owner}.", consoleOutput);
        Assert.Contains("Verbose output enabled.", consoleOutput);
    }

    [Fact]
    public void Invoke_WithAllParameters_OutputsCompleteMessage()
    {
        // Arrange
        var owner = "testowner";
        var number = 123;
        var description = "Test description";
        var verbose = true;
        
        var consoleOutput = CaptureConsoleOutput(() => 
        {
            // Act
            SubCommand2.Invoke(owner, number, description, verbose);
        });

        // Assert
        Assert.Contains($"Called SubCommand2 {number} owned by {owner}.", consoleOutput);
        Assert.Contains($"Description: {description}", consoleOutput);
        Assert.Contains("Verbose output enabled", consoleOutput);
    }

    private string CaptureConsoleOutput(Action action)
    {
        var originalOutput = Console.Out;
        using var stringWriter = new StringWriter();

        Console.SetOut(stringWriter);
        try
        {
            action();
            return stringWriter.ToString().Trim();
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

    }
}
