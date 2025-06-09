
using System;

namespace gh_kk.ClassIntegration;

public static class os_Integration
{

    public static Result RunConsoleProcess(string FileName, string Arguments, bool verbose = false)
    {
        try
        {
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = FileName,
                Arguments = Arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var process = System.Diagnostics.Process.Start(processStartInfo);

            if (process == null)
            {
                return new Result(string.Empty, "Failed to start the process.", -1);
            }

            string output = process.StandardOutput.ReadToEnd().Trim();
            string error = process.StandardError.ReadToEnd().Trim();

            process.WaitForExit();

            return new Result(output, error, process.ExitCode);
        }
        catch (Exception ex)
        {
            string errorMessage = $"An error occurred while running the process '{FileName}' with arguments '{Arguments}': {ex.Message}";
            return new Result(string.Empty, errorMessage, -1);
        }
    }


public sealed class Result
{
    public string Output { get; }
    public string Error { get; }
    public int ExitCode { get; }
    public bool Success => ExitCode == 0 && !string.IsNullOrWhiteSpace(Output);

    public Result (string output, string error, int exitCode)
    {
        Output = output ?? string.Empty;
        Error = error ?? string.Empty;
        ExitCode = exitCode;
    }
}
}
