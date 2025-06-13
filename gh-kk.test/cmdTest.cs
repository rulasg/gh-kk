using System;
using System.IO;
using System.Text;

namespace gh_kk.test;

public static class cmdTest
{
    public static string[] RunAndGetConsoleOutput(string[] args, params string[] userResponses)
    {

        //Output
        var _consoleOutput = new StringBuilder();
        Console.SetOut(new StringWriter(_consoleOutput));
        // var consoleOutput = new StringWriter();
        // Console.SetOut(consoleOutput);

        // Run the main method of the Program class
        gh_kk.Program.Main(args).GetAwaiter().GetResult();

        // Capture the console output
        var ret = _consoleOutput.ToString().Split(Environment.NewLine);

        // Return the console output as an array of strings
        return ret;
    }

}
