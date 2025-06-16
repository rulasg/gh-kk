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
            var verboseOption = new Option<bool>(
                aliases: new[] { "--verbose", "-v" },
                description: "Enable verbose output (global)");
            verboseOption.AddAlias("-v");
            GlobalOptions.AddOption("verbose", verboseOption);

            // Act - Get the "verbose" option that's added in Program.Main
            var option = GlobalOptions.GetOption<bool>("verbose");

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
            var exception = Assert.Throws<ArgumentException>(() => 
                GlobalOptions.GetOption<string>("nonexistent"));
        }

        [Fact]
        public void GetOption_ThrowsWhenOptionTypeDoesNotMatch()
        {
            // The "verbose" option is of type bool, trying to get it as int should throw
            var exception = Assert.Throws<ArgumentException>(() => 
                GlobalOptions.GetOption<int>("verbose"));
        }

        [Fact]
        public void AddOption_AddsOptionSuccessfully()
        {
            // Arrange
            var testOption = new Option<int>("--test", "Test option");

            // Act
            GlobalOptions.AddOption("test", testOption);
            var retrievedOption = GlobalOptions.GetOption<int>("test");

            // Assert
            Assert.NotNull(retrievedOption);
            Assert.Same(testOption, retrievedOption);
        }

        [Fact]
        public void AddOption_DoesNotOverwriteExistingOption()
        {
            // Arrange
            var originalOption = new Option<string>("--original", "Original option");
            var newOption = new Option<string>("--new", "New option");
            
            // Act
            GlobalOptions.AddOption("duplicate", originalOption);
            GlobalOptions.AddOption("duplicate", newOption); // Should not overwrite
            var retrievedOption = GlobalOptions.GetOption<string>("duplicate");

            // Assert
            Assert.Same(originalOption, retrievedOption);
            Assert.NotSame(newOption, retrievedOption);
        }
    }
}
