using System;
using Moq;
using Xunit;
using gh_kk.Interfaces;
using gh_kk.Integration;

namespace gh_kk.test.Integration
{
    public class GhIntegrationTests
    {
        private readonly Mock<IOsIntegration> _mockOsIntegration;
        private readonly GhIntegration _ghIntegration;

        public GhIntegrationTests()
        {
            _mockOsIntegration = new Mock<IOsIntegration>();
            _ghIntegration = new GhIntegration(_mockOsIntegration.Object);
        }

        [Fact]
        public void Constructor_NullOsIntegration_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => new GhIntegration(null));
            Assert.Equal("osIntegration", exception.ParamName);
        }

        [Fact]
        public void GetToken_Success_ReturnsToken()
        {
            // Arrange
            var expectedToken = "github_token_123456";
            _mockOsIntegration.Setup(o => o.RunConsoleProcess("gh", "auth token", false))
                .Returns(new Result(expectedToken, string.Empty, 0));

            // Act
            var result = _ghIntegration.GetToken(false);

            // Assert
            Assert.Equal(expectedToken, result);
            _mockOsIntegration.Verify(o => o.RunConsoleProcess("gh", "auth token", false), Times.Once);
        }

        [Fact]
        public void GetToken_SuccessWithVerbose_ReturnsToken()
        {
            // Arrange
            var expectedToken = "github_token_123456";
            _mockOsIntegration.Setup(o => o.RunConsoleProcess("gh", "auth token", true))
                .Returns(new Result(expectedToken, string.Empty, 0));


            // Act
            var result = _ghIntegration.GetToken(true);

            // Assert
            Assert.Equal(expectedToken, result);
            _mockOsIntegration.Verify(o => o.RunConsoleProcess("gh", "auth token", true), Times.Once);
        }

        [Fact]
        public void GetToken_Failure_ReturnsEmptyString()
        {
            // Arrange
            var errorMessage = "Authentication failed";
            _mockOsIntegration.Setup(o => o.RunConsoleProcess("gh", "auth token", false))
                .Returns(new Result(string.Empty, errorMessage, 1));
            // Act
            var result = _ghIntegration.GetToken(false);

            // Assert
            Assert.Equal(string.Empty, result);
            _mockOsIntegration.Verify(o => o.RunConsoleProcess("gh", "auth token", false), Times.Once);
        }

        [Fact]
        public void GetToken_ExceptionThrown_ReturnsEmptyString()
        {
            // Arrange
            _mockOsIntegration.Setup(o => o.RunConsoleProcess("gh", "auth token", false))
                .Throws(new Exception("Process execution failed"));

            // Act
            var result = _ghIntegration.GetToken(false);

            // Assert
            Assert.Equal(string.Empty, result);
            _mockOsIntegration.Verify(o => o.RunConsoleProcess("gh", "auth token", false), Times.Once);
        }
    }
}