
using System;
using gh_kk.Interfaces;

namespace gh_kk.Integration;

public class OsIntegration : IOsIntegration
{
    public gh_kk.Integration.Result RunConsoleProcess(string fileName, string arguments, bool verbose = false)
    {
        try
        {
            var processStartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
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
            string errorMessage = $"An error occurred while running the process '{fileName}' with arguments '{arguments}': {ex.Message}";
            return new Result(string.Empty, errorMessage, -1);
        }
    }
}
