using System;
using System.IO;
using System.Text;

namespace gh_kk.test;

internal sealed class CmdTest
{
    public string[] RunAndGetConsoleOutput(string[]? args, params string[] userResponses)
    {
        var originalOutput = Console.Out;
        StringWriter? stringWriter = null;

        try
        {
            var consoleOutput = new StringBuilder();
            stringWriter = new StringWriter(consoleOutput);
            Console.SetOut(stringWriter);

            Program.Main(args).GetAwaiter().GetResult();

            var output = consoleOutput.ToString();
            var result = output.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);

            return result;
        }
        finally
        {
            try
            {
                Console.SetOut(originalOutput);
            }
            finally
            {
                stringWriter?.Dispose();
            }
        }
    }
}
