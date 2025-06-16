using gh_kk.Integration;

namespace gh_kk.Interfaces;

public interface IOsIntegration
{
    gh_kk.Integration.Result RunConsoleProcess(string fileName, string arguments, bool verbose = false);
}
