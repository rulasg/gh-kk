using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;

namespace gh_kk.test;

class CmdTest
{
    public string[] RunAndGetConsoleOutput(string[]? args, params string[] userResponses)
    {

        var originalOutput = Console.Out;
        StringWriter? stringWriter = null;

        try
        {
            //Output
            var _consoleOutput = new StringBuilder();
            stringWriter = new StringWriter(_consoleOutput);
            Console.SetOut(stringWriter);

            // Run the main method of the Program class
            gh_kk.Program.Main(args).GetAwaiter().GetResult();

            // Capture the console output
            var output = _consoleOutput.ToString();
            var ret = output.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            // Return the console output as an array of strings
            return ret;
        }
        finally
        {
            try
            {
                // Restore the original console output
                Console.SetOut(originalOutput);
            }
            finally
            {
                // Dispose of the StringWriter to prevent resource leaks
                if (stringWriter != null)
                {
                    stringWriter.Dispose();
                }
            }
        }
    }

}
