using System;
using System.CommandLine;
using System.Collections;
using System.Linq;
using gh_kk;
using Xunit;

namespace gh_kk.test
{
    public class GlobalOptionsTest
    {
        [Fact]
        public void GetOption_ReturnsOptionWhenExists()
        {
            // Arrange
            var globalOptions = new GlobalOptions();
            var verboseOption = new Option<bool>(
                aliases: new[] { "--verbose", "-v" },
                description: "Enable verbose output (global)");
            verboseOption.AddAlias("-v");
            globalOptions.AddOption("verbose", verboseOption);

            // Act - Get the "verbose" option that's added in Program.Main
            var option = globalOptions.GetOption<bool>("verbose");

            // Assert
            Assert.NotNull(option);
            Assert.IsType<Option<bool>>(option);
            Assert.Contains("--verbose", option.Aliases);
            Assert.Contains("-v", option.Aliases);
        }

        [Fact]
        public void GetOption_ThrowsWhenOptionDoesNotExist()
        {
            // Act & Assert
            var globalOptions = new GlobalOptions();
            // Trying to get a non-existent option should throw an exception
            var exception = Assert.Throws<ArgumentException>(() => 
                globalOptions.GetOption<string>("nonexistent"));
        }

        [Fact]
        public void GetOption_ThrowsWhenOptionTypeDoesNotMatch()
        {
            // Arrange
            var globalOptions = new GlobalOptions();
            var verboseOption = new Option<bool>(
                aliases: new[] { "--verbose", "-v" },
                description: "Enable verbose output (global)");
            globalOptions.AddOption("verbose", verboseOption);

            // Act & Assert
            // The "verbose" option is of type bool, trying to get it as int should throw
            var exception = Assert.Throws<ArgumentException>(() =>
                globalOptions.GetOption<int>("verbose"));
        }

        [Fact]
        public void AddOption_AddsOptionSuccessfully()
        {
            // Arrange
            var globalOptions = new GlobalOptions();
            // Create a new option to add
            var testOption = new Option<string>("--test", "Test option");

            // Act
            globalOptions.AddOption("test", testOption);
            var retrievedOption = globalOptions.GetOption<string>("test");

            // Assert
            Assert.NotNull(retrievedOption);
            Assert.Same(testOption, retrievedOption);
        }

        [Fact]
        public void AddOption_DoesNotOverwriteExistingOption()
        {
            // Arrange
            var globalOptions = new GlobalOptions();

            // Create two options with the same name
            var originalOption = new Option<string>("--original", "Original option");
            var newOption = new Option<string>("--new", "New option");
            
            // Act
            globalOptions.AddOption("duplicate", originalOption);
            globalOptions.AddOption("duplicate", newOption); // Should not overwrite
            var retrievedOption = globalOptions.GetOption<string>("duplicate");

            // Assert
            Assert.Same(originalOption, retrievedOption);
            Assert.NotSame(newOption, retrievedOption);
        }
    }
}
