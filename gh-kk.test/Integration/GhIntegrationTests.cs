using System;
using Moq;
using gh_kk.Integration;
using gh_kk.Interfaces;

namespace gh_kk.test.Integration;

public class GhIntegrationTests
{
    [Fact]
    public void GetToken_ReturnsToken_WhenSuccessful()
    {
        // Arrange
        var mockOsIntegration = new Mock<IOsIntegration>();
        var expectedToken = "gho_test_token_123456";
        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth token", false))
            .Returns(new Result(expectedToken, string.Empty, 0));

        var ghIntegration = new GhIntegration(mockOsIntegration.Object);

        // Act
        var result = ghIntegration.GetToken(false);

        // Assert
        Assert.Equal(expectedToken, result);
        mockOsIntegration.Verify(m => m.RunConsoleProcess("gh", "auth token", false), Times.Once);
    }

    [Fact]
    public void GetToken_ReturnsEmpty_WhenCommandFails()
    {
        // Arrange
        var mockOsIntegration = new Mock<IOsIntegration>();
        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth token", false))
            .Returns(new Result(string.Empty, "error message", 1));

        var ghIntegration = new GhIntegration(mockOsIntegration.Object);

        // Act
        var result = ghIntegration.GetToken(false);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void GetToken_UsesHostnameFromEnvironment_WhenGhHostIsSet()
    {
        // Arrange
        var mockOsIntegration = new Mock<IOsIntegration>();
        var expectedToken = "gho_ghe_token_123456";
        Environment.SetEnvironmentVariable("GH_HOST", "remu.ghe.com");

        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth token --hostname remu.ghe.com", false))
            .Returns(new Result(expectedToken, string.Empty, 0));

        var ghIntegration = new GhIntegration(mockOsIntegration.Object);

        // Act
        var result = ghIntegration.GetToken(false);

        // Assert
        Assert.Equal(expectedToken, result);
        mockOsIntegration.Verify(m => m.RunConsoleProcess("gh", "auth token --hostname remu.ghe.com", false), Times.Once);

        // Cleanup
        Environment.SetEnvironmentVariable("GH_HOST", null);
    }

    [Fact]
    public void GetHostname_ReturnsGhHost_WhenEnvironmentVariableIsSet()
    {
        // Arrange
        var mockOsIntegration = new Mock<IOsIntegration>();
        var expectedHostname = "enterprise.github.com";
        Environment.SetEnvironmentVariable("GH_HOST", expectedHostname);

        var ghIntegration = new GhIntegration(mockOsIntegration.Object);

        // Act
        var result = ghIntegration.GetHostname(false);

        // Assert
        Assert.Equal(expectedHostname, result);

        // Cleanup
        Environment.SetEnvironmentVariable("GH_HOST", null);
    }

    [Fact]
    public void GetHostname_ParsesAuthStatus_WhenNoEnvironmentVariable()
    {
        // Arrange
        var mockOsIntegration = new Mock<IOsIntegration>();
        var authStatusOutput = @"github.com
  ✓ Logged in to github.com account testuser
  - Active account: true
  - Token: gho_************************************";

        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth status", false))
            .Returns(new Result(string.Empty, authStatusOutput, 0));

        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth token", false))
            .Returns(new Result("gho_test_default_token", string.Empty, 0));

        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth token --hostname github.com", false))
            .Returns(new Result("gho_test_default_token", string.Empty, 0));

        var ghIntegration = new GhIntegration(mockOsIntegration.Object);

        // Act
        var result = ghIntegration.GetHostname(false);

        // Assert
        Assert.Equal("github.com", result);
    }

    [Fact]
    public void GetHostname_DetectsGheInstance_WhenAuthStatusContainsMultipleHosts()
    {
        // Arrange
        var mockOsIntegration = new Mock<IOsIntegration>();
        var authStatusOutput = @"github.com
  ✓ Logged in to github.com account testuser
  - Active account: true
  - Token: gho_************************************

remu.ghe.com
  ✓ Logged in to remu.ghe.com account testuser
  - Active account: true
  - Token: gho_************************************";

        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth status", false))
            .Returns(new Result(string.Empty, authStatusOutput, 0));

        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth token", false))
            .Returns(new Result("gho_ghe_token_active", string.Empty, 0));

        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth token --hostname github.com", false))
            .Returns(new Result("gho_different_token", string.Empty, 0));

        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth token --hostname remu.ghe.com", false))
            .Returns(new Result("gho_ghe_token_active", string.Empty, 0));

        var ghIntegration = new GhIntegration(mockOsIntegration.Object);

        // Act
        var result = ghIntegration.GetHostname(false);

        // Assert
        Assert.Equal("remu.ghe.com", result);
    }

    [Fact]
    public void GetHostname_ReturnsDefaultGithubDotCom_WhenNoAuthStatus()
    {
        // Arrange
        var mockOsIntegration = new Mock<IOsIntegration>();
        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth status", false))
            .Returns(new Result(string.Empty, string.Empty, 1));

        var ghIntegration = new GhIntegration(mockOsIntegration.Object);

        // Act
        var result = ghIntegration.GetHostname(false);

        // Assert
        Assert.Equal("github.com", result);
    }

    [Fact]
    public void Constructor_ThrowsArgumentNullException_WhenOsIntegrationIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new GhIntegration(null!));
    }

    [Fact]
    public void GetToken_TrimsWhitespace_FromOutput()
    {
        // Arrange
        var mockOsIntegration = new Mock<IOsIntegration>();
        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth token", false))
            .Returns(new Result("  gho_token_with_spaces  \n", string.Empty, 0));

        var ghIntegration = new GhIntegration(mockOsIntegration.Object);

        // Act
        var result = ghIntegration.GetToken(false);

        // Assert
        Assert.Equal("gho_token_with_spaces", result);
    }

    [Fact]
    public void GetActiveUser_ReturnsEmpty_WhenTokenRetrievalFails()
    {
        // Arrange
        var mockOsIntegration = new Mock<IOsIntegration>();
        mockOsIntegration
            .Setup(m => m.RunConsoleProcess("gh", "auth token", false))
            .Returns(new Result(string.Empty, "error", 1));

        var ghIntegration = new GhIntegration(mockOsIntegration.Object);

        // Act
        var result = ghIntegration.GetActiveUser(false);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}
