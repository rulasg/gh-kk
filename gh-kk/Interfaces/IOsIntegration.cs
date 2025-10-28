using gh_kk.Integration;

namespace gh_kk.Interfaces;

public interface IOsIntegration
{
    Result RunConsoleProcess(string fileName, string arguments, bool verbose = false);
}
