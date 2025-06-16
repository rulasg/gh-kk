// #file:SubCommand2Tests.cs
using System;
using System.IO;
using Xunit;

namespace gh_kk.tests.Commands;

public class SubCommand2Tests
{
    [Fact]
    public void Invoke_WithBasicParameters_OutputsCorrectMessage()
    {
        // Arrange
        var owner = "testowner";
        var number = 123;
        string? description = null;
        var verbose = false;
        
        var consoleOutput = CaptureConsoleOutput(() => 
        {
            // Act
            SubCommand2.Invoke(owner, number, description, verbose);
        });

        // Assert
        Assert.Contains($"Called SubCommand2 {number} owned by {owner}.", consoleOutput);
        Assert.DoesNotContain("Description", consoleOutput);
        Assert.DoesNotContain("Verbose output enabled", consoleOutput);
    }

    [Fact]
    public void Invoke_WithDescription_IncludesDescriptionInOutput()
    {
        // Arrange
        var owner = "testowner";
        var number = 123;
        var description = "Test description";
        var verbose = false;
        
        var consoleOutput = CaptureConsoleOutput(() => 
        {
            // Act
            SubCommand2.Invoke(owner, number, description, verbose);
        });

        // Assert
        Assert.Contains($"Called SubCommand2 {number} owned by {owner}.", consoleOutput);
        Assert.Contains($"Description: {description}", consoleOutput);
        Assert.DoesNotContain("Verbose output enabled", consoleOutput);
    }

    [Fact]
    public void Invoke_WithVerboseFlag_IncludesVerboseMessageInOutput()
    {
        // Arrange
        var owner = "testowner";
        var number = 123;
        string? description = null;
        var verbose = true;
        
        var consoleOutput = CaptureConsoleOutput(() => 
        {
            // Act
            SubCommand2.Invoke(owner, number, description, verbose);
        });

        // Assert
        Assert.Contains($"Called SubCommand2 {number} owned by {owner}.", consoleOutput);
        Assert.Contains("Verbose output enabled", consoleOutput);
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
