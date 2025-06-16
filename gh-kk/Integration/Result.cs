using System;

namespace gh_kk.Integration;

public sealed class Result
{
    public string Output { get; }
    public string Error { get; }
    public int ExitCode { get; }
    public bool Success => ExitCode == 0 && !string.IsNullOrWhiteSpace(Output);

    public Result(string output, string error, int exitCode)
    {
        Output = output ?? string.Empty;
        Error = error ?? string.Empty;
        ExitCode = exitCode;
    }
}