using System.IO;
using System.Text;
using Xunit;


namespace gh_kk.test;

public class UnitTest1
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


}